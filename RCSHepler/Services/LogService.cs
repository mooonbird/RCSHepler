using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RCSHepler.Services
{
    class LogService
    {
        private static readonly string _logDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "YFAGV");

        /// <summary>
        /// 日志目录
        /// </summary>
        public static string LogDirectory { get; set; } = @"C:\Users\97916\Desktop\log";

        /// <summary>
        /// 日志轮转文件
        /// </summary>
        public static string LogPeriodFile { get; set; } 
            = Path.Combine(_logDirectory, "LogPeriod.txt");

        public static Dictionary<string, TimeSpan> Periods { get; set; } = new()
        {
            ["一周"] = new TimeSpan(7, 0, 0, 0),
            ["两周"] = new TimeSpan(14, 0, 0, 0),
            ["一月"] = new TimeSpan(30, 0, 0, 0),
            ["三月"] = new TimeSpan(90, 0, 0, 0),
        };

        static LogService()
        {
            LogRotationTimer.Elapsed += (s, e) =>
            {
                CleanLog(LogDirectory);
            };
        }

        /// <summary>
        /// 日志轮转周期
        /// </summary>
        public static TimeSpan LogRotationPeriod
        {
            get
            {
                if (File.Exists(LogPeriodFile))
                {
                    var key = File.ReadAllText(LogPeriodFile);

                    return Periods[key];
                }

                return Periods["三月"];
            }

            set
            {
                var p = Periods
                .Where(p => p.Value == value)
                .FirstOrDefault().Key;

                File.WriteAllText(LogPeriodFile, p);
            }
        }

        /// <summary>
        /// 日志轮转定时器
        /// </summary>
        public static System.Timers.Timer LogRotationTimer { get; set; } = new() 
        { 
            Interval = TimeSpan.FromSeconds(3).TotalMilliseconds
        };

        /// <summary>
        /// 日志清理
        /// </summary>
        public static void CleanLog(string path)
        {
            if (!Directory.Exists(path)) return;

            var directory = new DirectoryInfo(path);

            foreach (FileInfo file in directory.GetFiles())
            {
                if (file.Extension.ToLower() != ".txt") continue;

                if (DateTime.Now - file.LastWriteTime > LogRotationPeriod)
                {
                    try
                    {
                        file.Delete();
                    }
                    catch
                    {
                        continue;
                    }
                }
            }

            foreach (var dir in directory.GetDirectories())
            {
                CleanLog(dir.FullName);
            }
        }

        public static void Initialize()
        {
            LogRotationTimer.Start();

            if (!Directory.Exists(_logDirectory))
            {
                Directory.CreateDirectory(_logDirectory);
            }
        }
    }
}
