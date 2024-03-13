using LIBRA.Scan.ApiIntergration.ApiClients;
using LIBRA.Scan.ApiIntergration.ApiConstracts;
using LIBRA.Scan.Common.Enumerator;
using LIBRA.Scan.Common.Managers;
using LIBRA.Scan.Entities.Requests;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
    /// Interaction logic for ScanSettingManagerWindow.xaml
    /// </summary>
    public partial class ScanSettingManagerWindow : Window
    {
        private IRoleApiClient _roleApiClient;

        public ScanSettingManagerWindow()
        {
            _roleApiClient = new RoleApiClient();
            InitializeComponent();
        }

        private async void SftpConfig_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                VisibleSftpProgress();

                var userId = UserManager._userId;
                List<string> roles = await _roleApiClient.GetRolesByUserAsync(new GetRolesByUserRequest { UserId = userId });

                if (!roles.Any(ERoles.Admin.Contains))
                {
                    System.Windows.Forms.MessageBox.Show("Tài khoản không có quyền hạn truy cập!", "Thông báo!!");
                }
                else
                {
                    SftpConfigWindow sftpConfigWindow = new SftpConfigWindow();
                    sftpConfigWindow.Show();
                }
            }
            catch (HttpRequestException)
            {
                System.Windows.Forms.MessageBox.Show("Phần mềm hiện tại không kết nối được với Server!", "Thông báo!");
            }
            catch (JsonReaderException)
            {
                System.Windows.Forms.MessageBox.Show("Không nhận được thông tin ủy quyền của tài khoản!", "Thông báo!");
            }
            finally
            {
                CollapseSftpProgress();
            }
        }

        private void PortConfig_Click(object sender, RoutedEventArgs e)
        {
            PortConfigWindow portConfigWindow = new PortConfigWindow();
            portConfigWindow.Show();
        }

        private void CollapseSftpProgress()
        {
            txtSftpConfig.Visibility = Visibility.Visible;
            proSftp.Visibility = Visibility.Collapsed;
        }

        private void VisibleSftpProgress()
        {
            txtSftpConfig.Visibility = Visibility.Collapsed;
            proSftp.Visibility = Visibility.Visible;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
