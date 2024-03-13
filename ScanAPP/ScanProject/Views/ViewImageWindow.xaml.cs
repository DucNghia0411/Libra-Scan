
using Microsoft.EntityFrameworkCore.Migrations;
using ScanProject.Model;
using ScanProject.ViewModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Point = System.Windows.Point;

namespace ScanProject.Views
{
    /// <summary>
    /// Interaction logic for ViewImageWindơ.xaml
    /// </summary>
    public partial class ViewImageWindow : RibbonWindow
    {
        private ShowImageViewModel _imageViewModel;

        private Model.Image _selectedImage;

        private bool isDragging = false;

        private Point anchorPoint = new Point();
        public double FrameWitdh;
        public double FrameHeight;

        public System.Drawing.Image image;

        public ViewImageWindow(Model.Image selectedImage)
        {
            _selectedImage = selectedImage;

            InitializeComponent();
            this.DataContext = _imageViewModel = new ShowImageViewModel();

            image = System.Drawing.Image.FromFile(_selectedImage.ImagePath);
            ImageView.Source = _selectedImage.bitmapImage;

            BackPanel.MouseLeftButtonDown += new MouseButtonEventHandler(ImageView_MouseLeftButtonDown);
            BackPanel.MouseMove += new MouseEventHandler(ImageView_MouseMove);
            BackPanel.MouseLeftButtonUp += new MouseButtonEventHandler(ImageView_MouseLeftButtonUp);
            ImageView.MouseRightButtonDown += ImageView_MouseRightButtonDown;

            CropButton.IsEnabled = false;
            CutButton.IsEnabled = false;
        }

        private void ImageView_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
            selectionRectangle.Visibility = Visibility.Hidden;
        }

        private void CropButton_Click(object sender, RoutedEventArgs e)
        {
            //var uri = new System.Uri(SelectedImage.ImagePath);
            //var fullBitmap = new BitmapImage(uri);
            //Image tileSet = new Image();
            //ImageView.Source = new CroppedBitmap(fullBitmap, new Int32Rect(20, 50, 100, 100));
            Rect rect1 = new Rect(Canvas.GetLeft(selectionRectangle), Canvas.GetTop(selectionRectangle), selectionRectangle.Width, selectionRectangle.Height);
            System.Windows.Int32Rect rcFrom = new System.Windows.Int32Rect();
            rcFrom.X = (int)(rect1.X) ;
            rcFrom.Y = (int)(rect1.Y);
            rcFrom.Width = (int)((rect1.Width));
            rcFrom.Height = (int)((rect1.Height));
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(rcFrom.X, rcFrom.Y, rcFrom.Width, rcFrom.Height);
            //First we define a rectangle with the help of already calculated points  
            Bitmap OriginalImage = new Bitmap(image, (int)ImageView.ActualWidth, (int)ImageView.ActualHeight);
            //Original image  
            Bitmap _img = new Bitmap((int)rect1.Width, (int)rect1.Height);
            // for cropinf image  
            Graphics g = Graphics.FromImage(_img);
            // create graphics  
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.CompositingQuality = CompositingQuality.HighQuality;
            //set image attributes  
            g.DrawImage(OriginalImage, 0, 0, rect, GraphicsUnit.Pixel);
            BitmapImage bitmapImage = new BitmapImage();
            g.Dispose();
            using (MemoryStream memory = new MemoryStream())
            {
                _img.Save(memory, ImageFormat.Png);
                memory.Position = 0;
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
            }

            ImageView.Source = bitmapImage;
            ImageView.Width = _img.Width;
            ImageView.Height = _img.Height;
            image.Dispose();
            image = _img;
            selectionRectangle.Visibility = Visibility.Hidden;
        }

        private void ImageView_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Cursor == System.Windows.Input.Cursors.Hand)
                return;
            if (isDragging == false)
            {
                anchorPoint.X = e.GetPosition(BackPanel).X;
                anchorPoint.Y = e.GetPosition(BackPanel).Y;
                isDragging = true;
            }
        }

        private void ImageView_MouseMove(object sender, MouseEventArgs e)
        {
            if (Cursor == System.Windows.Input.Cursors.Hand)
                return;
            if (isDragging)
            {
                double x = e.GetPosition(BackPanel).X;
                double y = e.GetPosition(BackPanel).Y;
                selectionRectangle.SetValue(Canvas.LeftProperty, Math.Min(x, anchorPoint.X));
                selectionRectangle.SetValue(Canvas.TopProperty, Math.Min(y, anchorPoint.Y));
                selectionRectangle.Width = Math.Abs(x - anchorPoint.X);
                selectionRectangle.Height = Math.Abs(y - anchorPoint.Y);

                if (selectionRectangle.Visibility != Visibility.Visible)
                    selectionRectangle.Visibility = Visibility.Visible;
            }
        }

        private void ImageView_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (Cursor == System.Windows.Input.Cursors.Hand)
                return;
            if (isDragging)
            {
                isDragging = false;
                if (selectionRectangle.Width > 0)
                {
                    CropButton.Visibility = System.Windows.Visibility.Visible;
                    CropButton.IsEnabled = true;
                    CutButton.IsEnabled = true;
                }
                if (selectionRectangle.Visibility != Visibility.Visible)
                {
                    selectionRectangle.Visibility = Visibility.Visible; 
                    CropButton.IsEnabled = false;
                    CutButton.IsEnabled = false;
                }       
            }
            else
            {
                selectionRectangle.Visibility = Visibility.Hidden;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            BitmapSource bitmapSource = (BitmapSource)(ImageView.Source);
            try
            {
                System.Windows.Forms.DialogResult dr = System.Windows.Forms.MessageBox.Show("Bạn có chắc chắn lưu hình ảnh?","Lưu ý", System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question);
                if (dr == System.Windows.Forms.DialogResult.Yes)
                {
                    SaveBitmapSourceToFile(_selectedImage.ImagePath, bitmapSource);
                }
                else return;
            }
            catch (Exception)
            {
                MessageBox.Show("Ảnh đang được sử dụng. Vui lòng tắt ảnh trước khi thực hiện", "Lỗi", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return;
            }

            _selectedImage.bitmapImage = (BitmapImage)ImageView.Source;
            MessageBox.Show("Lưu ảnh thành công", "Thông báo", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);

        }
        public static void SaveBitmapSourceToFile(string filePath, BitmapSource image)
        {
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(image));
                encoder.Save(fileStream);
            }
        }

        private void CutButton_Click(object sender, RoutedEventArgs e)
        {

            if (ImageView.Source != null)
            {
                Rect rect1 = new Rect(Canvas.GetLeft(selectionRectangle), Canvas.GetTop(selectionRectangle), selectionRectangle.Width, selectionRectangle.Height);
                System.Windows.Int32Rect rcFrom = new System.Windows.Int32Rect();
                rcFrom.X = (int)(rect1.X);
                rcFrom.Y = (int)(rect1.Y);
                rcFrom.Width = (int)((rect1.Width));
                rcFrom.Height = (int)((rect1.Height));
                System.Drawing.Rectangle rect = new System.Drawing.Rectangle(rcFrom.X, rcFrom.Y, rcFrom.Width, rcFrom.Height);
                //First we define a rectangle with the help of already calculated points  
                Bitmap OriginalImage = new Bitmap(image,(int)ImageView.ActualWidth, (int)ImageView.ActualHeight);
                // for cropinf image  
                Graphics g = Graphics.FromImage(OriginalImage);
                
                // create graphics  
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;
                //set image attributes  
                System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Color.White,1);
                g.FillRectangle(System.Drawing.Brushes.White, rect);
                g.DrawRectangle(pen, rect);
                g.Dispose();
                BitmapImage bitmapImage = new BitmapImage();
                Bitmap bitmaptemp = new Bitmap(image.Width, image.Height);
                using (Graphics gp = Graphics.FromImage(bitmaptemp))
                {
                    gp.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    gp.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    gp.CompositingQuality = CompositingQuality.HighQuality;
                    gp.DrawImage(OriginalImage, 0, 0, image.Width, image.Height);
                    gp.Dispose();
                }
                using (MemoryStream memory = new MemoryStream())
                {

                    OriginalImage.Save(memory, ImageFormat.Png);
                    memory.Position = 0;
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = memory;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                    bitmapImage.Freeze();
                }
                ImageView.Source = bitmapImage;
                image.Dispose();
                image = OriginalImage;
                selectionRectangle.Visibility = Visibility.Hidden;
            }
        }

        private void RibbonWindow_Closed(object sender, EventArgs e)
        {
            image.Dispose();
        }
    }
}
