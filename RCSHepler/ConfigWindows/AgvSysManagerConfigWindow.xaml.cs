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
    /// AgvSysManagerConfigWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AgvSysManagerConfigWindow : Window
    {
        private const string SYS = "RCS:SysManagerAPIAddress";
        private const string GRPC = "RCS:SysManagerAPIGrpcAddress";

        public AgvSysConfigViewModel AgvSysConfig { get; set; }

        private string? ScheduleAddressCache { get; } = string.Empty;

        private string? GRPCAddressCache { get; } = string.Empty;

        public AgvSysManagerConfigWindow()
        {
            InitializeComponent();

            var redis = RedisService.CreateRedis();

            var schedule = redis.GetValue(SYS);
            var gRPC = redis.GetValue(GRPC);

            redis.Close();

            AgvSysConfig = new()
            {
                ScheduleAddress = schedule,
                GRPCAddress = gRPC
            };

            ScheduleAddressCache = schedule;
            GRPCAddressCache = gRPC;

            DataContext = AgvSysConfig;
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            var sys = AgvSysConfig.ScheduleAddress;
            
            IPEndPoint.Parse(sys!);

            var gRPC = AgvSysConfig.GRPCAddress;

            IPEndPoint.Parse(gRPC!);

            var redis = RedisService.CreateRedis();

            redis.SetValue(SYS, sys);
            redis.SetValue(GRPC, gRPC);

            redis.Close();

            MessageBox.Show(AgvSysConfig.ScheduleAddress);
        }

        private void Reset(object sender, RoutedEventArgs e)
        {
            AgvSysConfig.ScheduleAddress = ScheduleAddressCache;
            AgvSysConfig.GRPCAddress = GRPCAddressCache;
        }
    }
}
