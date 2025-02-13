using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace RCSHepler.Services
{
    class TimerService
    {
        /// <summary>
        /// 服务保活定时器
        /// </summary>
        public static System.Timers.Timer KeepAliveTimer { get; set; } = new() { Interval = TimeSpan.FromSeconds(3).TotalMilliseconds };

        public static void RefreshStatus(Control control)
        {
            if (KeepAliveTimer.Enabled)
            {
                control.Background = new SolidColorBrush(Colors.Green);
            }
            else
            {
                control.Background = new SolidColorBrush(Colors.Red);
            }
        }
    }
}
