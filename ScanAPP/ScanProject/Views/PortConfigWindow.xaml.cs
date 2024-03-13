using LIBRA.Scan.Common.Common;
using LIBRA.Scan.Common.Managers;
using LIBRA.Scan.Common.Models;
using LIBRA.Scan.Entities.LiteEntities;
using LIBRA.Scan.Services;
using LIBRA.Scan.Services.Constracts;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for PortConfigWindow.xaml
    /// </summary>
    public partial class PortConfigWindow : Window
    {
        private readonly IUrlService _portService;
        private int _hasPort = 0;

        public PortConfigWindow()
        {
            _portService = new URLService();
            InitializeComponent();
            GetSchemes();
            GetPorts();
            GetCurrentUrl();
            CheckHasPort();
        }

        private void GetSchemes()
        {
            List<string> schemes = new List<string> { "http", "https"};
            cboScheme.ItemsSource = schemes;
        }

        private void GetPorts()
        {
            List<ScanURL> scanURLs = _portService.GetAll()
                .Select(portConfig => new ScanURL
                {
                    Id = portConfig.id,
                    Url = portConfig.has_port == 1 ? $"{portConfig.scheme}:{portConfig.host_name}:{portConfig.port}" : $"{portConfig.scheme}:{portConfig.host_name}"
                })
                .ToList();

            cboList.ItemsSource = scanURLs;
            cboList.DisplayMemberPath = "Url";
            cboList.SelectedValuePath = "Id";
        }

        private void GetCurrentUrl()
        {
            txtCurrent.Text = UrlManager.Url ?? "Chưa có đường dẫn khả dụng";
        }

        private void cboListUrl_SelectionChanged(object sender, EventArgs e)
        {
            var cbo = (System.Windows.Controls.ComboBox)sender;
            var selectedItem = ValueConvert.ConvertToObject<ScanURL>(cbo.SelectedItem);

            txtCurrent.Text = selectedItem?.Url ?? "Chưa có đường dẫn khả dụng";
        }

        private void CheckHasPort()
        {
            chkHasPort.IsChecked = _hasPort != 0;
            txtPort.IsEnabled = _hasPort != 0;
        }

        private void chkHasPort_Checked(object sender, RoutedEventArgs e)
        {
            _hasPort = 1;
            txtPort.IsEnabled = true;
        }

        private void chkHasPort_Unchecked(object sender, RoutedEventArgs e)
        {
            _hasPort = 0;
            txtPort.IsEnabled = false;
        }

    }
}
