using RCSHepler.Services;
using Serilog;
using Serilog.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RCSHepler
{
    /// <summary>
    /// LogManagementPage.xaml 的交互逻辑
    /// </summary>
    public partial class LogManagementPage : Page
    {
        public TimeSpan CurrentLogRotationPeriod { get; set; }

        public LogManagementPage()
        {
            InitializeComponent();

            comBox.ItemsSource = LogService.Periods.Keys;

            comBox.SelectedItem = LogService.Periods
                .Where(p => p.Value == LogService.LogRotationPeriod)
                .FirstOrDefault().Key;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LogService.LogRotationPeriod = LogService.Periods[comBox.SelectionBoxItem.ToString()!];

            MessageBox.Show("【日志轮转】修改成功");
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }
    }
}
