using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RCSHepler.Services
{
    internal class FileService
    {
        /// <summary>
        /// 文件夹深度复制
        /// </summary>
        /// <param name="sourceDir"></param>
        /// <param name="destDir"></param>
        /// <param name="backupsDire"></param>
        public static void CopyDirRecursively(string sourceDir, string destDir, string backupsDire = null!)
        {
            if (Directory.Exists(sourceDir) && Directory.Exists(destDir))
            {
                DirectoryInfo sourceDireInfo = new DirectoryInfo(sourceDir);
                FileInfo[] fileInfos = sourceDireInfo.GetFiles();
                foreach (FileInfo fInfo in fileInfos)
                {
                    string sourceFile = fInfo.FullName;
                    string destFile = sourceFile.Replace(sourceDir, destDir);
                    if (backupsDire != null && File.Exists(destFile))
                    {
                        Directory.CreateDirectory(backupsDire);
                        string backFile = destFile.Replace(destDir, backupsDire);
                        File.Copy(destFile, backFile, true);
                    }
                    File.Copy(sourceFile, destFile, true);
                }
                DirectoryInfo[] direInfos = sourceDireInfo.GetDirectories();
                foreach (DirectoryInfo dInfo in direInfos)
                {
                    string sourceDire2 = dInfo.FullName;
                    string destDire2 = sourceDire2.Replace(sourceDir, destDir);
                    string backupsDire2 = null!;
                    if (backupsDire != null)
                    {
                        backupsDire2 = sourceDire2.Replace(sourceDir, backupsDire);
                    }
                    Directory.CreateDirectory(destDire2);
                    CopyDirRecursively(sourceDire2, destDire2, backupsDire2);
                }
            }
        }
    }
}
