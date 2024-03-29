﻿using Notification.Wpf;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace ScanProject.Views
{
    /// <summary>
    /// Interaction logic for CropImageWindow.xaml
    /// </summary>
    public partial class CropImageWindow : Window
    {
        public double _scale { get; set; }
        public double imageWidth { get; set; }
        public double imageHeight { get; set; }
        private Model.Image _image { get; }
        private BitmapSource _displayedImage;
        private Stack<BitmapSource> _undoStack = new Stack<BitmapSource>();
        private Stack<BitmapSource> _redoStack = new Stack<BitmapSource>();
        private readonly NotificationManager _notificationManager;

        private double _leftNumericValue = 0;
        private double _rightNumericValue = 0;
        private double _topNumericValue = 0;
        private double _bottomNumericValue = 0;

        private bool isDragging = false;
        private Image? draggedImage;
        private Point offset;

        public CropImageWindow(Model.Image image)
        {
            _notificationManager = new NotificationManager();
            InitializeComponent();
            _image = image;
            _displayedImage = _image.bitmapImage;
            DisplayImage();
            DataContext = this;
        }

        private void DisplayImage()
        {
            _scale = 0.8;
            imageHeight = _displayedImage.Height * _scale;
            imageWidth = _displayedImage.Width * _scale;
            image.Source = _displayedImage;
        }

        private void CropButton_Click(object sender, RoutedEventArgs e)
        {
            double left = Convert.ToDouble(txtLeft.Text);
            double right = Convert.ToDouble(txtRight.Text);
            double top = Convert.ToDouble(txtTop.Text);
            double bottom = Convert.ToDouble(txtBottom.Text);

            if (image.Source is BitmapSource bitmapSource)
            {
                int x = (int)left;
                int y = (int)top;
                int width = (int)(bitmapSource.PixelWidth - left - right);
                int height = (int)(bitmapSource.PixelHeight - top - bottom);

                CroppedBitmap croppedBitmap = new CroppedBitmap(bitmapSource, new Int32Rect(x, y, width, height));

                _undoStack.Push(_displayedImage);
                _displayedImage = croppedBitmap;
                image.Source = _displayedImage;
                _redoStack.Clear();
            }
        }

        private void UndoButton_Click(object sender, RoutedEventArgs e)
        {
            if (_undoStack.Count > 0)
            {
                _redoStack.Push(_displayedImage);
                _displayedImage = _undoStack.Pop();
                image.Source = _displayedImage;
            }
        }

        private void RedoButton_Click(object sender, RoutedEventArgs e)
        {
            if (_redoStack.Count > 0)
            {
                _undoStack.Push(_displayedImage);
                _displayedImage = _redoStack.Pop();
                image.Source = _displayedImage;
            }
        }

        private void SaveImageToOriginalPath(BitmapSource image)
        {
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));

            using (var fileStream = new FileStream(_image.ImagePath, FileMode.Create))
            {
                encoder.Save(fileStream);
            }
        }

        private void UpdateImageButton_Click(object sender, RoutedEventArgs e)
        {
            if (_displayedImage != null)
            {
                try
                {
                    SaveImageToOriginalPath(_displayedImage);
                }
                catch (Exception)
                {
                    var error = new NotificationContent
                    {
                        Title = "Thông báo!!",
                        Message = "Lưu hình ảnh thất bại!!",
                        Background = new SolidColorBrush(Colors.Red),
                        Foreground = new SolidColorBrush(Colors.White),
                    };
                    _notificationManager.Show(error);
                    return;
                }

                MainWindow mainWindow = (MainWindow)System.Windows.Application.Current.MainWindow;
                mainWindow.SelectedImage.bitmapImage = ConvertBitmapSourceToBitmapImage(_displayedImage);

                var success = new NotificationContent
                {
                    Title = "Thông báo!!",
                    Message = "Lưu hình ảnh thành công!!",
                    Background = new SolidColorBrush(Colors.Green),
                    Foreground = new SolidColorBrush(Colors.White),
                };
                _notificationManager.Show(success);
            }
            else
            {
                var content = new NotificationContent
                {
                    Title = "Thông báo!!",
                    Message = "Lưu hình ảnh thất bại!!",
                    Background = new SolidColorBrush(Colors.Red),
                    Foreground = new SolidColorBrush(Colors.White),
                };
                _notificationManager.Show(content);
            }
            _undoStack.Clear();
            _redoStack.Clear();
            this.Close();
        }

        private BitmapImage ConvertBitmapSourceToBitmapImage(BitmapSource bitmapSource)
        {
            var memoryStream = new System.IO.MemoryStream();
            BitmapEncoder encoder = new PngBitmapEncoder(); 
            encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
            encoder.Save(memoryStream);
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = memoryStream;
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad; 
            bitmapImage.EndInit();
            bitmapImage.Freeze();
            return bitmapImage;
        }

        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging && draggedImage != null)
            {
                Point mousePos = e.GetPosition(canvas);
                double offsetX = mousePos.X - offset.X;
                double offsetY = mousePos.Y - offset.Y;

                double newLeft = Canvas.GetLeft(draggedImage) + offsetX;
                double newTop = Canvas.GetTop(draggedImage) + offsetY;

                if (offsetY < 0)
                {
                    if (newTop < 0)
                    {
                        newTop = 0;
                    }
                }

                if (offsetX < 0)
                {
                    if (newLeft < 0)
                    {
                        newLeft = 0;
                    }
                }

                Canvas.SetLeft(draggedImage, newLeft);
                Canvas.SetTop(draggedImage, newTop);

                double rightEdge = newLeft + draggedImage.ActualWidth;
                double bottomEdge = newTop + draggedImage.ActualHeight;

                if (rightEdge > canvas.ActualWidth)
                {
                    canvas.Width = rightEdge;
                }

                if (bottomEdge > canvas.ActualHeight)
                {
                    canvas.Height = bottomEdge;
                }

                offset = mousePos;
            }
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            isDragging = true;
            offset = e.GetPosition(canvas);
            draggedImage = (Image)sender;
            draggedImage.CaptureMouse();
        }

        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
            if (draggedImage != null)
            {
                draggedImage.ReleaseMouseCapture();
            }
        }

        private void Canvas_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length > 0)
                {
                    draggedImage.Source = new System.Windows.Media.Imaging.BitmapImage(new System.Uri(files[0]));
                }
            }
        }

        private void UpdateLeftNumericValue()
        {
            txtLeft.Text = _leftNumericValue.ToString();
        }

        private void UpdateRightNumericValue()
        {
            txtRight.Text = _rightNumericValue.ToString();
        }

        private void UpdateTopNumericValue()
        {
            txtTop.Text = _topNumericValue.ToString();
        }

        private void UpdateBottomNumericValue()
        {
            txtBottom.Text = _bottomNumericValue.ToString();
        }

        private void IncreaseLeftButton_Click(object sender, RoutedEventArgs e)
        {
            _leftNumericValue++;

            if(_leftNumericValue < 0)
                _leftNumericValue = 0;

            UpdateLeftNumericValue();
        }

        private void DecreaseLeftButton_Click(object sender, RoutedEventArgs e)
        {
            _leftNumericValue--;

            if (_leftNumericValue < 0)
                _leftNumericValue = 0;

            UpdateLeftNumericValue();
        }

        private void IncreaseRightButton_Click(object sender, RoutedEventArgs e)
        {
            _rightNumericValue++;

            if (_rightNumericValue < 0)
                _rightNumericValue = 0;

            UpdateRightNumericValue();
        }

        private void DecreaseRightButton_Click(object sender, RoutedEventArgs e)
        {
            _rightNumericValue--;

            if (_rightNumericValue < 0)
                _rightNumericValue = 0;

            UpdateRightNumericValue();
        }

        private void IncreaseTopButton_Click(object sender, RoutedEventArgs e)
        {
            _topNumericValue++;

            if (_topNumericValue < 0)
                _topNumericValue = 0;

            UpdateTopNumericValue();
        }

        private void DecreaseTopButton_Click(object sender, RoutedEventArgs e)
        {
            _topNumericValue--;

            if (_topNumericValue < 0)
                _topNumericValue = 0;

            UpdateTopNumericValue();
        }

        private void IncreaseBottomButton_Click(object sender, RoutedEventArgs e)
        {
            _bottomNumericValue++;

            if (_bottomNumericValue < 0)
                _bottomNumericValue = 0;

            UpdateBottomNumericValue();
        }

        private void DecreaseBottomButton_Click(object sender, RoutedEventArgs e)
        {
            _bottomNumericValue--;

            if (_bottomNumericValue < 0)
                _bottomNumericValue = 0;

            UpdateBottomNumericValue();
        }
    }
}
