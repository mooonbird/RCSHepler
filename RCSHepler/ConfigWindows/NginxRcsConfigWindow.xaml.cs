using RCSHepler.Services;
using RCSHepler.ViewModels;
using System;
using System.Collections.Generic;
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
using Path = System.IO.Path;

namespace RCSHepler.ConfigWindows
{
    /// <summary>
    /// NginxRcsConfigWindow.xaml 的交互逻辑
    /// </summary>
    public partial class NginxRcsConfigWindow : Window
    {
        public NginxRcsViewModel NginxRcs { get; set; }

        public string? BackendAddressCache { get; set; }

        public string? PortCache { get; set; }

        public NginxRcsConfigWindow()
        {
            InitializeComponent();

            var nginxDir = RegisterService.GetDirectoryPath("dist")!;

            var distConfig = File.ReadAllText(Path.Combine(nginxDir, @"dist\config.js"));

            var nginxConfig = File.ReadAllText(Path.Combine(nginxDir, @"conf\nginx.conf"));


            NginxRcs = new NginxRcsViewModel()
            {
                BackendAddress = "2222",
                Port = "2222"
            };

            NginxRcs = NginxService.GetConfig();

            BackendAddressCache = NginxRcs.BackendAddress;
            PortCache = NginxRcs.Port;

            DataContext = NginxRcs;
        }

        private void Reset(object sender, RoutedEventArgs e)
        {
            NginxRcs.BackendAddress = BackendAddressCache;
            NginxRcs.Port = PortCache;
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            NginxService.UpdateConfig(NginxRcs.BackendAddress!, NginxRcs.Port!);
        }
    }
}
