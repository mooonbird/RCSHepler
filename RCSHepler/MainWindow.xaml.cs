using MySql.Data.MySqlClient;
using RCSHepler;
using RCSHepler.ViewModels;
using RCSHepler.Services;
using StackExchange.Redis;
using System.Configuration;
using System.Data;
using System.ServiceProcess;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Security.Policy;
using Hardcodet.Wpf.TaskbarNotification;

namespace RCSHepler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public readonly ServiceManagementPage _serviceManagementPage = new();
        private readonly AuthorizationInformationPage _authorizationInformationPage = new();
        private readonly SystemUpdatePage _systemUpdatePage = new();
        private readonly LogManagementPage _logManagementPage = new();

        public SoftwareInfoViewModel SoftwareInfoViewModel { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            mainFrame.Content = _serviceManagementPage;
            menuListBox.SelectedIndex = 0;

            menuListBox.ItemsSource = new List<ModuleMenu>()
            {
                new() { Name = "服务管理", SelectedIconSource = "Icons/ServiceManagementSelected.png", UnselectedIconSource = "Icons/ServiceManagement.png" },
                new() { Name = "系统升级", SelectedIconSource = "Icons/SystemUpdateSelected.png", UnselectedIconSource = "Icons/SystemUpdate.png" },
                new() { Name = "授权信息", SelectedIconSource = "Icons/AuthorizationInformationSelected.png", UnselectedIconSource = "Icons/AuthorizationInformation.png" },
                new() { Name = "日志管理", SelectedIconSource = "Icons/LogManagementSelected.png", UnselectedIconSource = "Icons/LogManagement.png" },
            };

            SoftwareInfoViewModel = new()
            {
                Url = "http://127.0.0.1:8081",
                AuthorizationStatus = "本机未授权",
                Version = LicenseService.ReadVersionInfo(),
            };

            if (LicenseService.VerifyLicenseWhenStartUp())
                SoftwareInfoViewModel.AuthorizationStatus = "成功";

            softwareInfo.DataContext = SoftwareInfoViewModel;

            _authorizationInformationPage.MainWindow = this;
            _systemUpdatePage.MainWindow = this;
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            //this.Close();
            Hide();
        }

        private void Minimize(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Maximize(object sender, RoutedEventArgs e)
        {
            
        }

        private void MenuListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (menuListBox.SelectedIndex)
            {
                case 0:
                    mainFrame.Content = _serviceManagementPage;
                    break;
                case 1:
                    mainFrame.Content = _systemUpdatePage;
                    break;
                case 2:
                    mainFrame.Content = _authorizationInformationPage;
                    break;
                case 3:
                    mainFrame.Content = _logManagementPage;
                    break;
                default:
                    break;
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Run(() =>
            {
                //MYSQL数据库连接测试
                var dbService = _serviceManagementPage._databaseServices
                        .FirstOrDefault(s => s.ServiceName == ConfigurationManager.AppSettings["DBServeName"]);

                if (dbService is not null)
                {
                    if(MySQLService.TryConnect(out string message))
                    {
                        dbService.ConnectionStatus = message;
                    }
                    else
                    {
                        MessageBox.Show(message);
                        dbService.ConnectionStatus = "连接失败";
                    }
                }

                //Redis连接测试
                var redisService = _serviceManagementPage._databaseServices
                        .FirstOrDefault(s => s.ServiceName == ConfigurationManager.AppSettings["RedisServerName"]);

                if (redisService is not null)
                {
                    try
                    {
                        if (RedisService.TryConnect(RedisService.GetConfigurationOption(), out string message))
                        {
                            redisService.ConnectionStatus = message;
                        }
                        else
                        {
                            MessageBox.Show(message);
                            redisService.ConnectionStatus = "连接失败";
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        redisService.ConnectionStatus = "连接失败";
                    }
                }
            });
        }

        private void NavigateURL(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
    }
}