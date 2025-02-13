using RCSHepler.Services;
using RCSHepler.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

namespace RCSHepler.ConfigWindows
{
    /// <summary>
    /// AgvInterfaceConfigWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AgvInterfaceConfigWindow : Window
    {
        private const string HTTP = "RCS:SysInterfaceApi";
        private const string TCP = "RCS:SysInterfaceTcp";

        public AgvInterfaceViewModel AgvInterface { get; set; }

        private string? SysInterfaceApiCache { get; } = string.Empty;

        private string? SysInterfaceTcpCache { get; } = string.Empty;

        public AgvInterfaceConfigWindow()
        {
            InitializeComponent();

            var redis = RedisService.CreateRedis();

            var http = redis.GetValue(HTTP);
            var tcp = redis.GetValue(TCP);

            redis.Close();

            AgvInterface = new()
            {
                SysInterfaceApi = http,
                SysInterfaceTcp = tcp
            };

            SysInterfaceApiCache = http;
            SysInterfaceTcpCache = tcp;

            DataContext = AgvInterface;
        }

        private void Reset(object sender, RoutedEventArgs e)
        {
            AgvInterface.SysInterfaceApi = SysInterfaceApiCache;
            AgvInterface.SysInterfaceTcp = SysInterfaceTcpCache;
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            var http = AgvInterface.SysInterfaceApi;

            IPEndPoint.Parse(http!);

            var tcp = AgvInterface.SysInterfaceTcp;

            IPEndPoint.Parse(tcp!);

            var redis = RedisService.CreateRedis();

            redis.SetValue(HTTP, http);
            redis.SetValue(TCP, tcp);

            redis.Close();

            MessageBox.Show(AgvInterface.SysInterfaceApi);
        }
    }
}
