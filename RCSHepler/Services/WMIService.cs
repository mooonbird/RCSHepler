using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace RCSHepler.Services
{
    public class WMIService
    {
        public static string GetWindowsInfo(string path, string name)
        {
            string result = string.Empty;

            ManagementClass mClass = new(path);

            ManagementObjectCollection mObjects = mClass.GetInstances();

            PropertyDataCollection properties = mClass.Properties;

            foreach (PropertyData propData in properties)
            {
                if (propData.Name == name)
                {
                    foreach (ManagementObject mobject in mObjects.Cast<ManagementObject>())
                    {
                        result = mobject.Properties[propData.Name].Value.ToString()!;
                    }
                }
            }

            return result;
        }

        public static string GetMacAddress(int i = 0)
        {
            string allmac = "FF;";
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                // 只获取已启用的、非虚拟和非回环接口的MAC地址
                if (nic.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                    !nic.Description.ToLowerInvariant().Contains("virtual") &&
                    !nic.Description.ToLowerInvariant().Contains("pseudo"))
                {
                    if (i == 0 && nic.OperationalStatus == OperationalStatus.Up)
                    {
                        return nic.GetPhysicalAddress().ToString();
                    }

                    allmac += nic.GetPhysicalAddress().ToString() + ";";
                }
            }

            return allmac;
        }

        public static Dictionary<string, string> GetCertificateAuthorityInfo()
        {
            var a = GetWindowsInfo(WMICONSTANT.Win32_BIOS, WMICONSTANT.SerialNumber);

            return new Dictionary<string, string>()
            {
                ["sysid"] = "sysrcs",
                ["bios"] = GetWindowsInfo(WMICONSTANT.Win32_BIOS, WMICONSTANT.SerialNumber),
                ["mac"] = GetMacAddress(1),
            };
        }
    }

    public class WMICONSTANT
    {
        public const string Win32_Processor = "Win32_Processor";
        public const string ProcessorId = "ProcessorId";

        public const string Win32_BIOS = "Win32_BIOS";
        public const string SerialNumber = "SerialNumber";

        public const string Win32_BaseBoard = "Win32_BaseBoard";
    }
}
