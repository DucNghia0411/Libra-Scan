using Azure;
using LIBRA.Scan.Common.Managers;
using LIBRA.Scan.Common.Models;
using LIBRA.Scan.Common.Status;
using LIBRA.Scan.Entities.LiteEntities;
using LIBRA.Scan.Entities.Requests;
using LIBRA.Scan.Services;
using LIBRA.Scan.Services.Constracts;
using Newtonsoft.Json;
using NTwain;
using NTwain.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Xml.Serialization;

namespace ScanProject.View
{
    /// <summary>
    /// Interaction logic for DeviceWindow.xaml
    /// </summary>
    public partial class DeviceWindow : Window
    {
        public TwainSession _twain;
        public MainWindow _mainWindow;
        public DataSource _dataSource;
        public DataSource _currentDataSource;
        public DeviceSetting deviceSetting;
        private IDeviceSettingService _deviceSettingService;

        public IEnumerable<DataSource> _dataSources = Enumerable.Empty<DataSource>();
        public IEnumerable<ScanSettingMode> _listMode = Enumerable.Empty<ScanSettingMode>();

        public DeviceWindow()
        {
            InitializeComponent();
            this.ResizeMode = ResizeMode.NoResize;
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            _deviceSettingService = new DeviceSettingService();
            deviceSetting = new DeviceSetting();

            //setting
            GetListMode();
            SettingCheckBox();
            CheckMode();
            DisableSettingCRUD();
        }

        private void ChooseDevice_Click(object sender, RoutedEventArgs e)
        {
            GetListDevice();
        }

        public void GetListDevice()
        {
            if(_twain != null)
            {
                _dataSources = _twain.GetSources().ToList();
                lbDevice.ItemsSource = _dataSources;
            }

            //GetLatestSetting();
        }

        private void GetLatestSetting()
        {
            var latestSetting = _deviceSettingService.GetLatest();
            if(latestSetting != null) 
            {
                deviceSetting.device_manufacturer = latestSetting.device_manufacturer;
                deviceSetting.device_name = latestSetting.device_name;
                deviceSetting.dpi = latestSetting.dpi;
                deviceSetting.depth = latestSetting.depth;
                deviceSetting.size = latestSetting.size;
                deviceSetting.duplex = latestSetting.duplex;

                var dataSource = _dataSources.FirstOrDefault(x => x.Name == deviceSetting.device_name);

                if(dataSource != null) 
                {
                    SettingDevice(dataSource);
                }
            }
        }

        private void btnCancle_Click(object sender, RoutedEventArgs e)
        {
            CheckMode();
            CheckBeforeClose();
            this.Visibility = Visibility.Hidden;
        }

        private void lblDevice_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataSource dataSource = (DataSource)lbDevice.SelectedItem;
            SettingDevice(dataSource);
        }

        private void SettingDevice(DataSource dataSource)
        {
            if (dataSource == null)
            {
                System.Windows.Forms.MessageBox.Show($"Bạn chưa chọn máy Scan", "Thông báo!!", MessageBoxButtons.OK);
                return;
            }

            _mainWindow.txtCurrentDevice.Text = dataSource.Name;
            txtCurrenDevice.Text = dataSource.Name;

            _mainWindow._dataSource = dataSource;
            _currentDataSource = dataSource;
        }

        private async void OpenSourceSetting_Click(object sender, RoutedEventArgs e)
        {
            VisibleProgress();
            DataSource dataSource = (DataSource)lbDevice.SelectedItem;
            if(dataSource == null)
            {
                System.Windows.Forms.MessageBox.Show("Vui lòng chọn thiết bị để cấu hình", "Thông báo!!", MessageBoxButtons.OK);
                CollapseProgress();
                return;
            }    

            if( _dataSource != null && dataSource.Id != _dataSource.Id && _dataSource.IsOpen)
            {
                _dataSource.Close();
            }

            if (!dataSource.IsOpen)
            {
                _dataSource = dataSource;
                await Task.Run(() =>dataSource.Open());
            }

            if(dataSource.IsOpen && _dataSource == null)
            {
                _dataSource = dataSource;
            }

            if (!_twain.IsSourceOpen)
            {
                System.Windows.Forms.MessageBox.Show("Vui lòng kiểm tra lại kết nối với thiết bị Scan của bạn!", "Thông báo!!", MessageBoxButtons.OK);
                CollapseProgress();
                return;
            }

            dataSource.Enable(SourceEnableMode.ShowUIOnly, false, IntPtr.Zero);
            CollapseProgress();
        }

        private void VisibleProgress()
        {
            this.txtOpenSource.Visibility = Visibility.Collapsed;
            proDevice.Visibility = Visibility.Visible;
            btnRefresh.IsEnabled = false;
            btnCancel.IsEnabled = false;
        }

        private void CollapseProgress()
        {
            this.txtOpenSource.Visibility = Visibility.Visible;
            proDevice.Visibility = Visibility.Collapsed;
            btnRefresh.IsEnabled = true;
            btnCancel.IsEnabled = true;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            CheckBeforeClose();

            if (e.Cancel == true)
                return;
            else
                Visibility = Visibility.Hidden;
        }

        private void CheckBeforeClose()
        {
            if (string.IsNullOrEmpty(this.txtCurrenDevice.Text) || string.IsNullOrWhiteSpace(this.txtCurrenDevice.Text))
            {
                var result = System.Windows.MessageBox.Show("Bạn chưa chọn thiết bị, có chắc chắn muốn thoát?", "Thông báo!!", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                switch (result)
                {
                    case MessageBoxResult.OK:
                        this.Visibility = Visibility.Hidden;
                        break;

                    case MessageBoxResult.Cancel:
                        return;
                }
            }
        }

        private void chkOneSide_Checked(object sender, RoutedEventArgs e)
        {
            chkTwoSide.IsChecked = false;
        }

        private void chkTwoSide_Checked(object sender, RoutedEventArgs e)
        {
            chkOneSide.IsChecked = false;
        }

        private void SettingCheckBox()
        {
            if(ScanSettingManager._IsDuplex == false)
            {
                chkOneSide.IsChecked = true;
                chkTwoSide.IsChecked = false;
            }
            else
            {
                chkOneSide.IsChecked = false;
                chkTwoSide.IsChecked = true;
            }
        }

        private void GetListMode()
        {
            _listMode = new List<ScanSettingMode>
            {
                new ScanSettingMode { Mode = "Mặc định", Id = (int)ScanMode.Default},
                new ScanSettingMode { Mode = "Tùy chỉnh", Id = (int)ScanMode.Custom}
            };

            cboMode.ItemsSource = _listMode;
            cboMode.DisplayMemberPath = "Mode";
            cboMode.SelectedValuePath = "Id";
            
            if(ScanSettingManager._IsDefault == true)
                cboMode.SelectedValue = (int)ScanMode.Default;
            else
                cboMode.SelectedValue = (int)ScanMode.Custom;
        }

        private void cboMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ScanSettingMode selectedMode = (ScanSettingMode)cboMode.SelectedItem;
            if(selectedMode != null)
            {
                if (selectedMode.Id == (int)ScanMode.Default)
                    ScanSettingManager._IsDefault = true;
                else
                    ScanSettingManager._IsDefault = false;
            }
            CheckMode();
        }

        private void CheckMode()
        {
            if (ScanSettingManager._IsDefault == true)
            {
                btnCustom.IsEnabled = false;
                btnOpenSource.IsEnabled = true;
            }
            else
            {
                btnCustom.IsEnabled = true;
                btnOpenSource.IsEnabled = false;
            }

            ShowHideSettingOption(false, false, false, false, false);
            ShowHideSettingButton(false, false, false);
        }

        private void DisableSettingCRUD()
        {
            btnSettingCancel.IsEnabled = false;
            btnSettingSave.IsEnabled = false;
            btnSettingEdit.IsEnabled = false;
        }

        private void btnCustom_Click(object sender, RoutedEventArgs e)
        {
            if(_currentDataSource == null)
            {
                System.Windows.Forms.MessageBox.Show("Vui lòng chọn Thiết bị trước khi thực hiện tùy chỉnh!", "Thông báo!!", MessageBoxButtons.OK);
                return;
            }

            cboMode.IsEnabled = false;

            ShowHideDeviceButton(false, false, false);
            ShowHideSettingButton(false, true, true);

            var src = _currentDataSource;

            if (!src.IsOpen)
            {
                src.Open();
            }

            if (!src.IsOpen)
            {
                System.Windows.Forms.MessageBox.Show("Thiết bị hiện tại không khả dụng!", "Thông báo!!", MessageBoxButtons.OK);

                grpDPI.Header = "DPI (Không khả dụng)";
                grpDepth.Header = "Độ sâu màu sắc (Không khả dụng)";
                grpSize.Header = "Kích cỡ (Không khả dụng)";

                ShowHideSettingButton(false, false, false);
                ShowHideSettingOption(false, false, false, false, false);
                ShowHideDeviceButton(true, true, false);
                cboMode.IsEnabled = true;
                return;
            }

            LoadSourceCaps(src);
        }

        private void btnSettingEdit_Click(object sender, RoutedEventArgs e)
        {
            ShowHideSettingButton(true, false, true);
            ShowHideSettingOption(true, true, true, true, true);
        }

        private void btnSettingCancel_Click(object sender, RoutedEventArgs e)
        {
            ShowHideSettingButton(false, false, false);
            ShowHideSettingOption(false, false, false, false, false);
            ShowHideDeviceButton(true, true, false);

            cboMode.IsEnabled = true;
        }

        private void btnSettingSave_Click(object sender, RoutedEventArgs e)
        {
            bool isDuplex = false;

            ShowHideSettingButton(false, true, true);
            ShowHideSettingOption(false, false, false, false, false);

            if (chkOneSide.IsChecked == true)
            {
                ScanSettingManager.SetDuplex(false);
                isDuplex = false;
            }

            if (chkTwoSide.IsChecked == true)
            {
                ScanSettingManager.SetDuplex(true);
                isDuplex = true;
            }

            if (_twain.State == (int)ScanState.TransferReady)
            {
                object depth = cboDepth.SelectedItem;
                ScanSettingManager.SetDepth(depth);

                object dpi = cboDPI.SelectedItem;
                ScanSettingManager.SetDpi(dpi);

                object size = cboSize.SelectedItem;
                ScanSettingManager.SetSize(size);

                try
                {
                    SaveSetting(depth, dpi, size, isDuplex);
                }
                catch (Exception) { }
            }
        }

        public void LoadSourceCaps(DataSource src)
        {
            if (src.Capabilities.ICapPixelType.IsSupported)
            {
                LoadDepth(src.Capabilities.ICapPixelType);
                grpDepth.Header = "Độ sâu màu sắc";
            }

            if (src.Capabilities.ICapXResolution.IsSupported && src.Capabilities.ICapYResolution.IsSupported)
            {
                LoadDPI(src.Capabilities.ICapXResolution);
                grpDPI.Header = "DPI";
            }

            if (src.Capabilities.ICapSupportedSizes.IsSupported)
            {
                LoadPaperSize(src.Capabilities.ICapSupportedSizes);
                grpSize.Header = "Kích cỡ";
            }
        }

        private void LoadDPI(ICapWrapper<TWFix32> cap)
        {
            var list = cap.GetValues().Where(dpi => (dpi % 50) == 0).ToList();
            cboDPI.ItemsSource = list;
            var cur = cap.GetCurrent();
            if (list.Contains(cur))
            {
                cboDPI.SelectedItem = cur;
            }
        }

        private void LoadPaperSize(ICapWrapper<SupportedSize> cap)
        {
            var list = cap.GetValues().ToList();
            cboSize.ItemsSource = list;
            var cur = cap.GetCurrent();
            if (list.Contains(cur))
            {
                cboSize.SelectedItem = cur;
            }
        }

        private void LoadDepth(ICapWrapper<PixelType> cap)
        {
            var list = cap.GetValues().ToList();
            cboDepth.ItemsSource = list;
            var cur = cap.GetCurrent();
            if (list.Contains(cur))
            {
                cboDepth.SelectedItem = cur;
            }
        }

        private void ShowHideDeviceButton(bool refresh, bool custom, bool openSource)
        {
            btnRefresh.IsEnabled = refresh;
            btnCustom.IsEnabled = custom;
            btnOpenSource.IsEnabled = openSource;
        }

        private void ShowHideSettingOption(bool dpi, bool depth, bool size, bool oneSize, bool twoSize)
        {
            cboDPI.IsEnabled = dpi;
            cboDepth.IsEnabled = depth;
            cboSize.IsEnabled = size;
            chkOneSide.IsEnabled = oneSize;
            chkTwoSide.IsEnabled = twoSize;
        }

        private void ShowHideSettingButton(bool save, bool edit, bool cancel)
        {
            btnSettingSave.IsEnabled = save;
            btnSettingEdit.IsEnabled = edit;
            btnSettingCancel.IsEnabled = cancel;
        }

        private void SaveSetting(object depth, object dpi, object size, bool duplex)
        {
            CreateDeviceSettingRequest request = new CreateDeviceSettingRequest()
            {
                manufacturer = _currentDataSource.Manufacturer,
                device_name = _currentDataSource.Name,
                depth = JsonConvert.SerializeObject(depth),
                dpi = Convert.ToInt32(dpi.ToString()),
                size = JsonConvert.SerializeObject(size),
                duplex = duplex ? 1 : 0
            };
            _deviceSettingService.Create(request);
        }
    }
}
