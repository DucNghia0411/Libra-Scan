using LIBRA.Scan.Services;
using NTwain;
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
    /// Interaction logic for LoadSessionWindow.xaml
    /// </summary>
    public partial class LoadSessionWindow : Window
    {
        private ISetUp _setUp;
        private DataSource? _dataSource;
        private TwainSession? _twain;

        public LoadSessionWindow()
        {
            _setUp = new SetUp();
            InitializeComponent();
            Loaded += Window_Loaded;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Topmost = true;
            Loaded -= Window_Loaded;
            SessionInitialization();
        }

        private async void SessionInitialization()
        {
            try
            {
                await Task.Delay(1000);
                await Task.Run(() => GetTwainSession());

                await Task.Delay(1000);
                await Task.Run(() => GetDataSource());

                await Task.Delay(1000);
                await Task.Run(async () =>
                {
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        LoadProgessMain(true, false);
                    });
                });

                await Task.Delay(2000);
                Close();
            }
            catch (Exception)
            {
                Close();
            }
        }

        private async Task GetTwainSession()
        {
            await Task.Run(() => _twain = _setUp.GetTwainSession());

            if (_twain != null)
            {
                if (!_twain.IsDsmOpen)
                {
                    pnlFailedTwain.Visibility = Visibility.Visible;
                }

                await Task.Run(async () =>
                {
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
                        mainWindow._twain = _twain;
                        LoadProgessTwain(true, false, false);
                    });
                });
            }
            else
            {
                await Task.Run(async () =>
                {
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        LoadProgessTwain(false, true, false);
                    });
                });
            }
        }

        private async Task GetDataSource()
        {
            var sources = await Task.Run(() => _setUp.GetDataSource());
            if (sources.Count() > 0)
            {
                if (_twain != null)
                {
                    await Task.Run(async () =>
                    {
                        await Application.Current.Dispatcher.InvokeAsync(async () =>
                        {
                            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
                            _dataSource = _twain.First();
                            mainWindow._dataSource = _dataSource;
                            mainWindow._dataSource.Open();

                            LoadProgessSource(true, false, false);
                        });
                    });
                }
            }
            else
            {
                await Task.Run(async () =>
                {
                    await Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        LoadProgessSource(false, true, false);
                    });
                });
            }
        }

        private void LoadProgessMain(bool sucess, bool progress)
        {
            if (sucess)
                pnlMainDone.Visibility = Visibility.Visible;
            else
                pnlMainDone.Visibility = Visibility.Collapsed;

            if (progress)
                pnlMainProgress.Visibility = Visibility.Visible;
            else
                pnlMainProgress.Visibility = Visibility.Collapsed;
        }

        private void LoadProgessTwain(bool sucess, bool faild, bool progress)
        {
            if(sucess)
                pnlFinishTwain.Visibility = Visibility.Visible;
            else
                pnlFinishTwain.Visibility = Visibility.Collapsed;

            if (faild)
                pnlFailedTwain.Visibility= Visibility.Visible;
            else
                pnlFailedTwain.Visibility = Visibility.Collapsed;

            if (progress)
                pnlProcessTwain.Visibility = Visibility.Visible;
            else
                pnlProcessTwain.Visibility = Visibility.Collapsed;
        }

        private void LoadProgessSource(bool sucess, bool faild, bool progress)
        {
            if (sucess)
                pnlFinishSource.Visibility = Visibility.Visible;
            else
                pnlFinishSource.Visibility = Visibility.Collapsed;

            if (faild)
                pnlFailedSource.Visibility = Visibility.Visible;
            else
                pnlFailedSource.Visibility = Visibility.Collapsed;

            if (progress)
                pnlProcessSource.Visibility = Visibility.Visible;
            else
                pnlProcessSource.Visibility = Visibility.Collapsed;
        }
    }
}
