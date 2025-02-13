using Hardcodet.Wpf.TaskbarNotification;
using Mysqlx.Crud;
using RCSHepler.Services;
using RCSHepler.ViewModels;
using Serilog;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Threading;
using System.Windows;

namespace RCSHepler
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("【未处理异常】" + e.Exception.Message, "Exception Sample", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }

        private Mutex? mutex;
        private TaskbarIcon? _taskbarIcon;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            mutex = new Mutex(true, @"Global\robotphoenix.com RCSHelper", out bool createdNew);

            if (!createdNew)
            {
                MessageBox.Show("另一个应用程序实例正在运行。再见！");

                Environment.Exit(0);
            }

            Log.Logger = new LoggerConfiguration()
            .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

            LogService.Initialize();

            _taskbarIcon = (TaskbarIcon)FindResource("taskBarIcon");
            _taskbarIcon.DataContext = new TaskBarIconViewModel();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _taskbarIcon?.Dispose();

            mutex?.ReleaseMutex();
            mutex?.Dispose();

            base.OnExit(e);
        }
    }
}
