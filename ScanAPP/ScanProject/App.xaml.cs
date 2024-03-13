using System;
using LIBRA.Scan.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ScanProject.View;
using System.Windows;
using LIBRA.Scan.Common.StartupHelpers;
using LIBRA.Scan.ApiIntergration.ApiClients;
using LIBRA.Scan.ApiIntergration.ApiConstracts;
using Notification.Wpf;
using ScanProject.Views;
using System.Windows.Threading;
using System.IO;
using System.Threading.Tasks;

namespace ScanProject
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IHost? _host { get; private set; }

        public App() 
        {
            WriteToFile("Start App Host at: " + DateTime.Now);
            try
            {
                _host = Host.CreateDefaultBuilder()
                    .ConfigureServices((context, services) =>
                    {
                        services.AddSingleton<MainWindow>();
                        services.AddTransient<IUserApiClient, UserApiClient>();
                        services.AddTransient<IRoleApiClient, RoleApiClient>();
                        services.AddTransient<INotificationManager, NotificationManager>();
                    }).Build();
            }
            catch (Exception ex)
            {
                WriteToFile("App host error at: " + DateTime.Now + ex.ToString());
            }
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await _host!.StartAsync();
            MainWindow mainWindow = _host.Services.GetRequiredService<MainWindow>();
            //LoginWindow loginWindow = new LoginWindow();
            mainWindow.Show();
            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            WriteToFile("Stop at: " + DateTime.Now);
            await _host!.StopAsync();
            base.OnExit(e); 
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            Shutdown();
        }

        private void minimizeButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.WindowState = WindowState.Minimized;
        }

        void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs args)
        {
            WriteToFile("Error DispatcherUnhandled at: " + DateTime.Now + $"{args.Exception}");
            args.Handled = true;
            Environment.Exit(0);
        }

        void App_UnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            WriteToFile("Error UnhandledException at: " + DateTime.Now + $"{args}");
            Environment.Exit(0);
        }

        void App_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs args)
        {
            WriteToFile("Error UnobservedTaskException at: " + DateTime.Now + $"{args.Exception}");
            Environment.Exit(0);
        }

        public void WriteToFile(string Message)
        {
            string userFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string path = Path.Combine(userFolderPath, "Log_TimeCheck");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = Path.Combine(userFolderPath, "Log_TimeCheck", "ServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt");
            if (!File.Exists(filepath))
            {
                // Create a file to write to.   
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
        }
    }
}
