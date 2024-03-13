using LIBRA.Scan.ApiIntergration.ApiClients;
using LIBRA.Scan.ApiIntergration.ApiConstracts;
using LIBRA.Scan.Common.Common;
using LIBRA.Scan.Common.Enumerator;
using LIBRA.Scan.Common.Managers;
using LIBRA.Scan.Entities.LiteEntities;
using LIBRA.Scan.Entities.Requests;
using LIBRA.Scan.Services;
using LIBRA.Scan.Services.Constracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
    /// Interaction logic for SftpConfigWindow.xaml
    /// </summary>
    public partial class SftpConfigWindow : Window
    {
        private readonly ISftpService _sftpService;
        private readonly IRoleApiClient _roleApiClient;
        private long? _configId;

        public SftpConfigWindow()
        {
            InitializeComponent();

            _sftpService = new SftpService();
            _roleApiClient = new RoleApiClient();

            GetSftpConfig();
            StartCheck();
        }

        public void GetSftpConfig()
        {
            SftpConfig sftpConfig = _sftpService.Get();

            if (sftpConfig != null) 
            {
                _configId = sftpConfig.id;
                txtHostName.Text = sftpConfig.host_name;
                txtPortNumber.Text = sftpConfig.port_number.ToString();
                txtUserName.Text = sftpConfig.user_name;
                txtPassword.Text = sftpConfig.password;
                txtHostKey.Text = sftpConfig.host_key;
                txtFolderPath.Text = sftpConfig.folder_path;
            }
        }

        private void btnEditSFTP_Click(object sender, RoutedEventArgs e)
        {
            txtHostName.IsEnabled = true;
            txtHostKey.IsEnabled = true;
            txtPortNumber.IsEnabled = true;
            txtUserName.IsEnabled = true;
            txtPassword.IsEnabled = true;
            txtHostKey.IsEnabled = true;
            txtFolderPath.IsEnabled = true;

            btnEditSFTP.IsEnabled = false;
            btnSaveSFTP.IsEnabled = true;
            btnCancelSFTP.IsEnabled = true;
            btnCloseSFTP.IsEnabled = false;
        }

        private void btnCancelSFTP_Click(object sender, RoutedEventArgs e)
        {
            txtHostName.IsEnabled = false;
            txtHostKey.IsEnabled = false;
            txtPortNumber.IsEnabled = false;
            txtUserName.IsEnabled = false;
            txtPassword.IsEnabled = false;
            txtHostKey.IsEnabled = false;
            txtFolderPath.IsEnabled = false;

            btnEditSFTP.IsEnabled = true;
            btnSaveSFTP.IsEnabled = false;
            btnCancelSFTP.IsEnabled = false;
            btnCloseSFTP.IsEnabled = true;
        }

        private void btnSaveSFTP_Click(object sender, RoutedEventArgs e)
        {
            if (_configId == null)
            {
                System.Windows.MessageBox.Show($"Cấu hình SFTP bạn muốn cập nhật không tồn tại!", "Thông báo!", MessageBoxButton.OK);
                return;
            }

            long configId = (long)_configId;

            SftpUpdateRequest request = new SftpUpdateRequest()
            {
                id = configId,
                host_name = txtHostName.Text,
                port_number = Convert.ToInt32(txtPortNumber.Text),
                user_name = txtUserName.Text,
                password = txtPassword.Text,
                host_key = txtHostKey.Text,
                folder_path = txtFolderPath.Text
            };

            if(!_sftpService.Update(request))
            {
                System.Windows.Forms.MessageBox.Show("Cập nhật cấu hình SFTP thất bại!", "Thông báo!!");
                return;
            }

            System.Windows.Forms.MessageBox.Show("Cập nhật cấu hình SFTP thành công!", "Thông báo!!");

            GetSftpConfig();

            txtHostName.IsEnabled = false;
            txtHostKey.IsEnabled = false;
            txtPortNumber.IsEnabled = false;
            txtUserName.IsEnabled = false;
            txtPassword.IsEnabled = false;
            txtHostKey.IsEnabled = false;
            txtFolderPath.IsEnabled = false;

            btnEditSFTP.IsEnabled = true;
            btnSaveSFTP.IsEnabled = false;
            btnCancelSFTP.IsEnabled = false;
            btnCloseSFTP.IsEnabled = true;
        }

        private void StartCheck()
        {
            txtHostName.IsEnabled = false;
            txtHostKey.IsEnabled = false;
            txtPortNumber.IsEnabled = false;
            txtUserName.IsEnabled = false;
            txtPassword.IsEnabled = false;
            txtHostKey.IsEnabled = false;
            txtFolderPath.IsEnabled = false;

            btnEditSFTP.IsEnabled = true;
            btnSaveSFTP.IsEnabled = false;
            btnCancelSFTP.IsEnabled = false;
            btnCloseSFTP.IsEnabled = true;
        }

        private void btnCloseSFTP_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
