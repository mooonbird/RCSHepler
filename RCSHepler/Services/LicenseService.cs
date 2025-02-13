using Org.BouncyCastle.Bcpg.OpenPgp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RCSHepler.Services
{
    internal class LicenseService
    {
        private static readonly string _versionDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "YFAGV");

        /// <summary>
        /// 版本信息
        /// </summary>
        public static string VersionFile { get; set; }
            = Path.Combine(_versionDirectory, "version.txt");

        public static string ReadVersionInfo()
        {
            string? result;

            if (!File.Exists(VersionFile))
            {
                result = "未知";
            }
            else
            {
                result = File.ReadAllText(VersionFile);
            }

            return result;
        }

        public static void WriteVersionInfo(string version)
        {
            File.WriteAllText(VersionFile, version);
        }

        /// <summary>
        /// 获取许可证数据
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetLicenseData(string fileName)
        {
            string result = string.Empty;

            var lines = File.ReadLines(fileName).ToList();

            byte[] priKeyByte = null!;

            var keyMatch = Regex.Match(lines[0], @"prikey=(.+?)(?=\s|$)");

            if (keyMatch.Success)
            {
                priKeyByte = Convert.FromBase64String(keyMatch.Groups[1].Value);
            }

            //获取私钥
            var priKey = Encoding.UTF8.GetString(priKeyByte);

            //获取密文
            var messageMatch = Regex.Match(lines[1], @"encrypto=(.+?)(?=\s|$)");

            if (keyMatch.Success)
            {
                result = Decrypt(messageMatch.Groups[1].Value, priKey);
            }

            return result;
        }

        /// <summary>
        /// RSA解密
        /// </summary>
        /// <param name="data"></param>
        /// <param name="privateKey"></param>
        /// <returns></returns>
        public static string Decrypt(string data, string privateKey)
        {
            using RSACryptoServiceProvider rsa = new();

            rsa.FromXmlString(privateKey);

            byte[] decrypted = rsa.Decrypt(Convert.FromBase64String(data), false);

            return Encoding.UTF8.GetString(decrypted);
        }

        /// <summary>
        /// 验证许可证
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool VerifyLicense(string fileName)
        {
            var data = GetLicenseData(fileName);

            bool result = true;

            var pattern = @"(.+?)(?=\s|$)";

            var bios = Regex.Match(data, $"bios={pattern}").Groups[1].Value;

            var mac = Regex.Match(data, $"mac={pattern}").Groups[1].Value;

            var info = WMIService.GetCertificateAuthorityInfo();

            if(info["bios"] != bios)
                result = false;

            if (!info["mac"].Split(';').Intersect(mac.Split(";")).Any(i => !string.IsNullOrEmpty(i)))
            {
                result = false;
            }

            MoveLicense(fileName);

            return result;
        }

        /// <summary>
        /// 放置许可证
        /// </summary>
        /// <param name="sourceFileName"></param>
        public static void MoveLicense(string sourceFileName)
        {
            var rootDir = RegisterService.GetDirectoryPath("SysManager");

            var fileName = Path.GetFileName(sourceFileName);

            File.Move(sourceFileName, Path.Combine(rootDir, fileName));
        }

        /// <summary>
        /// 启动时验证许可证
        /// </summary>
        public static bool VerifyLicenseWhenStartUp()
        {
            try
            {
                var rootDir = RegisterService.GetDirectoryPath("SysManager");

                var file = Path.Combine(rootDir, "Licence.key");

                if (!File.Exists(file))
                    throw new FileNotFoundException("许可证未找到", file);

                return VerifyLicense(file);
            }
            catch
            {
                return false;
            }
        }
    }
}
