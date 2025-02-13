using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RCSHepler.Services
{
    /// <summary>
    /// 注册表
    /// </summary>
    internal class RegisterService
    {
        private const string PATH = @"SYSTEM\CurrentControlSet\Services\";

        /// <summary>
        /// 获取注册表的ImagePath值
        /// </summary>
        /// <param name="appKey"></param>
        /// <returns></returns>
        public static string GetImagePath(string appKey)
        {
            string result = string.Empty;

            string? serviceName = ConfigurationManager.AppSettings[appKey];

            using RegistryKey? key = Registry.LocalMachine.OpenSubKey(PATH + serviceName);

            if (key is not null && key.GetValue("ImagePath") is string value)
            {
                result = value;
            }

            return result;
        }

        /// <summary>
        /// 获取安装文件的根目录
        /// </summary>
        /// <returns></returns>
        public static string GetRCSRootDir()
        {
            var dirName = Path.GetDirectoryName(GetImagePath("SysInterface"))!;

            return Directory.GetParent(dirName)!.FullName;
        }

        /// <summary>
        /// 获取特定服务的根目录
        /// </summary>
        /// <param name="appKey"></param>
        /// <returns></returns>
        public static string GetDirectoryPath(string appKey)
        {
            return Path.GetDirectoryName(GetImagePath(appKey))!.Replace("\"", "");
        }
    }
}
