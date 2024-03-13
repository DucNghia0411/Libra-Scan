using Azure;
using Azure.Core;
using LIBRA.Scan.ApiIntergration.ApiClients;
using LIBRA.Scan.ApiIntergration.ApiConstracts;
using LIBRA.Scan.ApiIntergration.Ultilities;
using LIBRA.Scan.Common.Config;
using LIBRA.Scan.Common.Encode;
using LIBRA.Scan.Common.Manager;
using LIBRA.Scan.Common.Managers;
using LIBRA.Scan.Common.StartupHelpers;
using LIBRA.Scan.Data.EFs;
using LIBRA.Scan.Entities.Requests;
using LIBRA.Scan.Entities.Respones;
using LIBRA.Scan.Services;
using LIBRA.Scan.Services.Constracts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using NTwain;
using ScanProject.View;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.Eventing.Reader;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using static System.Collections.Specialized.BitVector32;
using Application = System.Windows.Application;

namespace ScanProject.Views
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly ScanAppContext _context;
        public ISetUp _setUp;

        //api clients
        public IUserApiClient _userApiClient;
        private IAuthService _authService;

        bool IsLogin = false;

        public LoginWindow()
        {
            InitializeComponent();
            _context = new ScanAppContext();
            _userApiClient = new UserApiClient();
            _authService = new AuthService();
            _setUp = new SetUp();
            Loaded += Window_Loaded;
        }

        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            VisibleProgress();

            string? latestToken = String.Empty;
            if (CheckLoginField() != "")
            {
                System.Windows.Forms.MessageBox.Show(CheckLoginField(), "Thông báo!!", MessageBoxButtons.OK);
                CollapseProgress();
                return;
            }

            await Task.Run(async () =>
            {
                await UpdateLoginLabelAsync("Tiến hành đăng nhập...");
            });

            AuthRequest request = new AuthRequest()
            {
                UserName = txtUserName.Text,
                Password = txtPassword.Password
            };

            Task<RequestResponse> result = _userApiClient.Authenticate(request);
            RequestResponse respone = await result;

            if (respone.StatusCode != Code.OK)
            {
                System.Windows.Forms.MessageBox.Show($"{respone.Message} - Mã lỗi: {(int)respone.StatusCode}", "Thông báo!!", MessageBoxButtons.OK);
                CollapseProgress();

                await Task.Run(async () =>
                {
                    await UpdateLoginLabelAsync("");
                });
                return;
            }

            SessionManager.SetToken(respone.Content);

            DateTime now = DateTime.Now;
            TokenCreateRequest tokenCreate = new TokenCreateRequest()
            {
                token = respone.Content,
                created_date = now.ToString()
            };

            long tokenId = await _authService.Create(tokenCreate);
            if (tokenId == 0)
            {
                System.Windows.Forms.MessageBox.Show("Có lỗi xảy ra trong quá trình đăng nhập, vui lòng đăng nhập lại!", "Thông báo!!", MessageBoxButtons.OK);
                CollapseProgress();

                await Task.Run(async () =>
                {
                    await UpdateLoginLabelAsync("");
                });
                return;
            }

            latestToken = respone.Content;

            ClaimsPrincipal claimsPrincipal = DecodeJwtToken(latestToken);
            UserManager.SetUserId(long.Parse(claimsPrincipal.Claims.First(c => c.Type == TokenConfig.Audience).Value));
            IsLogin = true;

            await Task.Delay(1000);
            await Task.Run(async () =>
            {
                await UpdateLoginLabelAsync("Đăng nhập thành công!");
            });

            await Task.Delay(1000);
            await Task.Run(async () =>
            {
                await UpdateLoginLabelAsync("Thực hiện chuyển tiếp...");
            });

            await Task.Delay(1000);
            await Task.Run(async () =>
            {
                await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                });
            });

            CollapseProgress();
            this.Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!IsLogin)
            {
                var result = System.Windows.MessageBox.Show("Bạn có chắc chắn muốn thoát khỏi hệ thống?", "Thông báo!!", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                switch (result)
                {
                    case MessageBoxResult.OK:
                        System.Windows.Application.Current.Shutdown();
                        break;

                    case MessageBoxResult.Cancel:
                        e.Cancel = true;
                        break;
                }
            }
        }

        private static ClaimsPrincipal DecodeJwtToken(string jwtToken)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwtSecurityToken = tokenHandler.ReadJwtToken(jwtToken);
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(jwtSecurityToken.Claims));
            return claimsPrincipal;
        }

        private bool CheckTokenExpired(string latestToken)
        {
            if (String.IsNullOrEmpty(latestToken))
                return true;

            var claimsPrincipal = DecodeJwtToken(latestToken);
            long expired = long.Parse(claimsPrincipal.Claims.First(c => c.Type == TokenConfig.Expired).Value);
            var expirationDate = DateTimeOffset.FromUnixTimeSeconds(expired).UtcDateTime;

            if (expirationDate <= DateTime.UtcNow)
                return false;
            else
                return true;
        }

        private string CheckLoginField()
        {
            string notification = string.Empty;
            if (txtUserName.Text.Trim() == "")
                notification += "Tên đăng nhập không được để trống! \n";
            if (txtPassword.Password.Trim() == "")
                notification += "Mật khẩu không được để trống! \n";
            return notification;
        }

        private void VisibleProgress()
        {
            this.txtLogin.Visibility = Visibility.Collapsed;
            proLogin.Visibility = Visibility.Visible;
        }

        private void CollapseProgress()
        {
            this.txtLogin.Visibility = Visibility.Visible;
            proLogin.Visibility = Visibility.Collapsed;
        }

        private async Task UpdateLoginLabelAsync(string newContent)
        {
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                lblLogin.Content = newContent;
            });
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            VisibleProgress();

            string? latestToken = String.Empty;
            bool isExpired = false;
            await Task.Run(() => latestToken = _authService.GetLatestToken());
            await Task.Run(() => isExpired = CheckTokenExpired(latestToken));

            if (!isExpired)
            {
                await Task.Run(async () =>
                {
                    await UpdateLoginLabelAsync("Tiến hành đăng nhập...");
                });

                ClaimsPrincipal claimsPrincipal = DecodeJwtToken(latestToken);
                UserManager.SetUserId(long.Parse(claimsPrincipal.Claims.First(c => c.Type == TokenConfig.Audience).Value));

                await Task.Delay(1000);
                await Task.Run(async () =>
                {
                    await UpdateLoginLabelAsync("Đăng nhập thành công!");
                });

                await Task.Delay(1000);
                await Task.Run(async () =>
                {
                    await UpdateLoginLabelAsync("Thực hiện chuyển tiếp...");
                });

                await Task.Delay(1000);
                await Task.Run(async () =>
                {
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        MainWindow mainWindow = new MainWindow();
                        mainWindow.Show();
                    });
                });
                IsLogin = true;
                CollapseProgress();
                this.Close();
            }

            await Task.Delay(1000);
            await Task.Run(async () =>
            {
                await UpdateLoginLabelAsync("Hết phiên đăng nhập!");
            });
            CollapseProgress();
        }
    }
}
