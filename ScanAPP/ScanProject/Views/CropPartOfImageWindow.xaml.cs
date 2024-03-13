using LIBRA.Scan.Helpers;
using Notification.Wpf;
using PdfSharp.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace ScanProject.Views
{
    /// <summary>
    /// Interaction logic for CropPartOfImageWindow.xaml
    /// </summary>
    public partial class CropPartOfImageWindow : Window
    {
        private Model.Image _image { get; }
        private readonly NotificationManager _notificationManager;

        private bool isDragging = false;
        private Point startPoint;
        private Rectangle currentRectangle;

        public double imageWidth { get; }
        private BitmapImage _bitmapImage;
        private double _scale = 0.5;

        private Stack<BitmapImage> _undoStack = new Stack<BitmapImage>();
        private Stack<BitmapImage> _redoStack = new Stack<BitmapImage>();
        private BitmapImage _displayedImage;
        private RenderTargetBitmap _mergeImageBitmap;

        public CropPartOfImageWindow(Model.Image image)
        {
            _notificationManager = new NotificationManager();
            InitializeComponent();
            _image = image;
            _bitmapImage = image.bitmapImage;
            _displayedImage = image.bitmapImage;
            mainImage.Source = image.bitmapImage;
            imageWidth = _bitmapImage.Width * _scale;
            mainImage.MouseLeftButtonDown += MainImage_MouseLeftButtonDown;
            mainImage.MouseMove += MainImage_MouseMove;
            overlayCanvas.MouseLeftButtonUp += MainImage_MouseLeftButtonUp;
            DataContext = this;
        }

        private void MainImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (currentRectangle != null)
                overlayCanvas.Children.Remove(currentRectangle);

            startPoint = e.GetPosition(mainImage);
            currentRectangle = new Rectangle
            {
                Stroke = Brushes.Red,
                StrokeThickness = 2,
                Fill = new SolidColorBrush(Color.FromArgb(50, 255, 0, 0))
            };

            Canvas.SetLeft(currentRectangle, startPoint.X);
            Canvas.SetTop(currentRectangle, startPoint.Y);
            overlayCanvas.Children.Add(currentRectangle);
            isDragging = true;
        }

        private void MainImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
        }

        private void MainImage_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isDragging || currentRectangle == null)
                return;

            Point newPoint = e.GetPosition(mainImage);
            double width = newPoint.X - startPoint.X;
            double height = newPoint.Y - startPoint.Y;

            if (width > 0 && height > 0)
            {
                Canvas.SetLeft(currentRectangle, startPoint.X);
                Canvas.SetTop(currentRectangle, startPoint.Y);
                currentRectangle.Width = width;
                currentRectangle.Height = height;
            }
        }

        private void DrawRectangleButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentRectangle != null)
            {
                CheckImageFormat();
                int bitmapWidth = (int)_displayedImage.Width;
                int bitmapHeight = (int)_displayedImage.Height;
                int pixelWidth = (int)_displayedImage.PixelWidth;
                int pixelHeight = (int)_displayedImage.PixelHeight;

                Color backgroundColor = GetBackgroundColor();
                currentRectangle.Fill = new SolidColorBrush(backgroundColor);
                DrawingVisual visual = new DrawingVisual();
                using (DrawingContext context = visual.RenderOpen())
                {
                    context.DrawImage(_displayedImage, new Rect(0, 0, bitmapWidth, bitmapHeight));
                    context.DrawRectangle(new SolidColorBrush(backgroundColor), null, new Rect(startPoint.X / _scale, startPoint.Y / _scale, currentRectangle.Width / _scale, currentRectangle.Height / _scale));
                }
                RenderTargetBitmap renderBitmap = new RenderTargetBitmap(pixelWidth, pixelHeight, _displayedImage.DpiX, _displayedImage.DpiY, PixelFormats.Pbgra32);
                renderBitmap.Render(visual);
                _mergeImageBitmap = renderBitmap;

                BitmapImage bitmapImage = BitmapHelpers.ConvertRenderTargetBitmapToBitmapImage(renderBitmap);
                double scalePercent = 0.4;
                ScaleTransform scaleTransform = new ScaleTransform(scalePercent, scalePercent);
                TransformedBitmap transformedBitmap = new TransformedBitmap(bitmapImage, scaleTransform);

                _undoStack.Push(_displayedImage);
                _displayedImage = bitmapImage;
                mainImage.Source = transformedBitmap;
                _redoStack.Clear();
            }
        }

        private void CheckImageFormat()
        {
            System.Windows.Media.PixelFormat pixelFormat = _displayedImage.Format;
            if (pixelFormat == PixelFormats.BlackWhite)
            {
                MessageBoxResult result = System.Windows.MessageBox.Show($"Định dạng của hình ảnh không phù hợp với chức năng, bạn có muốn cập nhật?", "Xác nhận", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {
                    int bitmapWidth = (int)_bitmapImage.Width;
                    int bitmapHeight = (int)_bitmapImage.Height;
                    int pixelWidth = (int)_bitmapImage.PixelWidth;
                    int pixelHeight = (int)_bitmapImage.PixelHeight;

                    DrawingVisual drawingVisual = new DrawingVisual();
                    using (DrawingContext drawingContext = drawingVisual.RenderOpen())
                    {
                        drawingContext.DrawImage(_displayedImage, new Rect(0, 0, bitmapWidth, bitmapHeight));
                    }
                    RenderTargetBitmap targetBitmap = new RenderTargetBitmap(pixelWidth, pixelHeight, _bitmapImage.DpiX, _bitmapImage.DpiY, PixelFormats.Pbgra32);
                    targetBitmap.Render(drawingVisual);

                    BitmapImage bitmapImage = BitmapHelpers.ConvertRenderTargetBitmapToBitmapImage(targetBitmap);
                    double scalePercent = 0.4;
                    ScaleTransform scaleTransform = new ScaleTransform(scalePercent, scalePercent);
                    TransformedBitmap transformedBitmap = new TransformedBitmap(bitmapImage, scaleTransform);
                    _displayedImage = bitmapImage;
                    mainImage.Source = transformedBitmap;

                    MainWindow mainWindow = (MainWindow)System.Windows.Application.Current.MainWindow;
                    mainWindow.SelectedImage.bitmapImage = BitmapHelpers.ConvertBitmapSourceToBitmapImage(_displayedImage);
                    var content = new NotificationContent
                    {
                        Title = "Thông báo!!",
                        Message = "Cập nhật thành công!!",
                        Background = new SolidColorBrush(Colors.Green),
                        Foreground = new SolidColorBrush(Colors.White),
                    };
                    _notificationManager.Show(content);
                    this.Close();
                    CropPartOfImageWindow window = new CropPartOfImageWindow(_image);
                    window.Show();
                }
                else
                    return;

                overlayCanvas.Children.Remove(currentRectangle);
            }
        }

        private void SaveImage_Click(object sender, RoutedEventArgs e)
        {
            BitmapEncoder encoder = new PngBitmapEncoder();
            string filePath = _image.ImagePath;
            encoder.Frames.Add(BitmapFrame.Create(_mergeImageBitmap));
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                encoder.Save(fileStream);
            }
            MainWindow mainWindow = (MainWindow)System.Windows.Application.Current.MainWindow;
            mainWindow.SelectedImage.bitmapImage = BitmapHelpers.ConvertBitmapSourceToBitmapImage(_displayedImage);
            var content = new NotificationContent
            {
                Title = "Thông báo!!",
                Message = "Lưu ảnh thành công!!",
                Background = new SolidColorBrush(Colors.Green),
                Foreground = new SolidColorBrush(Colors.White),
            };
            _notificationManager.Show(content);
            _undoStack.Clear();
            _redoStack.Clear();
            this.Close();
        }

        private Color GetBackgroundColor()
        {
            BitmapSource bitmapSource = (BitmapSource)_bitmapImage;

            int pixelX = (int)(bitmapSource.PixelWidth / 2);
            int pixelY = (int)(bitmapSource.PixelHeight / 2);

            CroppedBitmap croppedBitmap = new CroppedBitmap(bitmapSource, new Int32Rect(pixelX, pixelY, 1, 1));
            byte[] pixel = new byte[4];
            croppedBitmap.CopyPixels(pixel, 4, 0);

            return Color.FromArgb(pixel[3], pixel[2], pixel[1], pixel[0]);
        }

        private void UndoButton_Click(object sender, RoutedEventArgs e)
        {
            if (_undoStack.Count > 0)
            {
                _redoStack.Push(_displayedImage);
                _displayedImage = _undoStack.Pop();
                mainImage.Source = _displayedImage;
                overlayCanvas.Children.Remove(currentRectangle);
            }
        }

        private void RedoButton_Click(object sender, RoutedEventArgs e)
        {
            if (_redoStack.Count > 0)
            {
                _undoStack.Push(_displayedImage);
                _displayedImage = _redoStack.Pop();
                mainImage.Source = _displayedImage;
                overlayCanvas.Children.Remove(currentRectangle);
            }
        }
    }
}
