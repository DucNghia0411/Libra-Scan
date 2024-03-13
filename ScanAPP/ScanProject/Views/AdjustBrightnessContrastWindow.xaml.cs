using AForge.Imaging.Filters;
using LIBRA.Scan.Helpers;
using Notification.Wpf;
using System;
using System.Collections.Generic;
using System.Drawing;
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
using System.Windows.Threading;

namespace ScanProject.Views
{
    /// <summary>
    /// Interaction logic for AdjustBrightnessContrastWindow.xaml
    /// </summary>
    public partial class AdjustBrightnessContrastWindow : Window
    {
        private Model.Image _image { get; }
        private readonly NotificationManager _notificationManager;
        private BitmapSource _adjustImage;
        private DispatcherTimer _debounceTimer;
        private BitmapImage _adjustedBitmapImage;

        public AdjustBrightnessContrastWindow(Model.Image image)
        {
            _notificationManager = new NotificationManager();
            _debounceTimer = new DispatcherTimer();
            InitializeComponent();
            _image = image;
            _adjustImage = _image.bitmapImage;
            imgAdjust.Source = _adjustImage;
            _debounceTimer.Interval = TimeSpan.FromMilliseconds(500);
            _debounceTimer.Tick += DebounceTimer_Tick;

            BrightnessSlider.ValueChanged += Slider_ValueChanged;
            ContrastSlider.ValueChanged += Slider_ValueChanged;
            DataContext = this;
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            BrightnessValueLabel.Text = BrightnessSlider.Value.ToString();
            ContrastValueLabel.Text = ContrastSlider.Value.ToString();
            _debounceTimer.Stop();
            _debounceTimer.Start();
        }

        private void DebounceTimer_Tick(object sender, EventArgs e)
        {
            ApplyAdjustments();
            _debounceTimer.Stop();
        }

        private void ApplyAdjustments()
        {
            Bitmap imageBitmap = BitmapHelpers.ConvertBitmapSourceToBitmap(_adjustImage);
            Bitmap bitmap = AForge.Imaging.Image.Clone(imageBitmap, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            BrightnessCorrection brightnessFilter = new BrightnessCorrection();
            brightnessFilter.AdjustValue = (int)BrightnessSlider.Value;
            Bitmap adjustedBrightnessImage = brightnessFilter.Apply(bitmap);

            ContrastCorrection contrastFilter = new ContrastCorrection();
            contrastFilter.Factor = (int)ContrastSlider.Value;
            Bitmap adjustedContrastImage = contrastFilter.Apply(adjustedBrightnessImage);

            _adjustedBitmapImage = BitmapHelpers.ConvertBitmapToBitmapImage(adjustedContrastImage);
            imgAdjust.Source = _adjustedBitmapImage;

            bitmap.Dispose();
            adjustedBrightnessImage.Dispose();
            adjustedContrastImage.Dispose();
        }

        private void SaveAdjustedImageButton_Click(object sender, RoutedEventArgs e)
        {
            if (_adjustedBitmapImage != null)
            {
                try
                {
                    BitmapHelpers.SaveImageToOriginalPath(_adjustedBitmapImage, _image.ImagePath);
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
                mainWindow.SelectedImage.bitmapImage = BitmapHelpers.ConvertBitmapSourceToBitmapImage(_adjustedBitmapImage);

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
            this.Close();
        }

        private void Revert_Click(object sender, RoutedEventArgs e)
        {
            BrightnessSlider.Value = 0;
            ContrastSlider.Value = 0;
            ApplyAdjustments();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = (MainWindow)System.Windows.Application.Current.MainWindow;
            mainWindow.ListIMGSelected.Remove(_image);
            this.Close();
        }
    }
}
