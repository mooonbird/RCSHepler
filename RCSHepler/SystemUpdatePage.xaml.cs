using Microsoft.Win32;
using RCSHepler.Services;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
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
using Path = System.IO.Path;

namespace RCSHepler
{
    /// <summary>
    /// ServiceManagementPage.xaml 的交互逻辑
    /// </summary>
    public partial class SystemUpdatePage : Page
    {
        public MainWindow MainWindow { get; set; } = null!;

        public SystemUpdatePage()
        {
            InitializeComponent();
            
            var text = File.ReadAllText("logs/log.txt");

            FlowDocument doc = new();

            Paragraph p = new(new Run(text))
            {
                FontSize = 14
            };
            doc.Blocks.Add(p);

            //p = new Paragraph(new Run("The ultimate programming greeting!"));
            //p.FontSize = 14;
            //p.FontStyle = FontStyles.Italic;
            //p.TextAlignment = TextAlignment.Left;
            //p.Foreground = Brushes.Gray;
            //doc.Blocks.Add(p);

            fdViewer.Document = doc;
        }

        private async void Update(object sender, RoutedEventArgs e)
        {
            var sourceZipFile = fileFullName.Text;

            if (string.IsNullOrEmpty(sourceZipFile))
            {
                MessageBox.Show("升级包未指定");
                return;
            }

            //停止服务
            TimerService.KeepAliveTimer.Stop();

            BackgroundService.StopAll();

            var rootDir = RegisterService.GetRCSRootDir();

            var tempDir = Path.Combine(Path.GetDirectoryName(sourceZipFile)!, "temp");

            progressBar.Foreground = new SolidColorBrush(Colors.Green);
            progressBar.IsIndeterminate = true;

            try
            {
                await Task.Run(() =>
                {
                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                    ZipFile.ExtractToDirectory(sourceZipFile, tempDir, Encoding.GetEncoding("gb2312"), true);

                    var directories = new List<string>() { "SysInterface", "SysManager", "dist" };

                    var targetDirectories = Directory.GetDirectories(tempDir).Select(d => Path.GetFileName(d));

                    if (!targetDirectories.Intersect(directories).Any())
                    {
                        throw new Exception("安装包有误：未发现任何服务的安装目录");
                    }

                    foreach (var sourceDir in Directory.GetDirectories(tempDir))
                    {
                        var directoryName = Path.GetFileName(sourceDir);

                        if (directoryName == "SysInterface")
                        {
                            FileService.CopyDirRecursively(sourceDir, sourceDir.Replace(tempDir, rootDir));
                        }
                        
                        if (directoryName == "SysManager")
                        {
                            FileService.CopyDirRecursively(sourceDir, sourceDir.Replace(tempDir, rootDir));
                        }

                        if (directoryName == "dist")
                        {
                            var path = @"ExtenServer\nginx";
                            var distDir = Path.Combine(rootDir, path);

                            FileService.CopyDirRecursively(sourceDir, sourceDir.Replace(tempDir, distDir));
                        }
                    }

                    //更新版本信息
                    var file = File.ReadAllText(Directory.GetFiles(tempDir).First());

                    var doc = JsonDocument.Parse(file, new JsonDocumentOptions { AllowTrailingCommas = true });

                    var version = doc.RootElement[0].GetProperty("VersionCode").GetString();

                    MainWindow.SoftwareInfoViewModel.Version = version;

                    Log.Information("升级版本，VersionCode：{@version}", version);
                    LicenseService.WriteVersionInfo(version!);
                });
            }
            catch (Exception ex)
            {
                progressBar.Foreground = new SolidColorBrush(Colors.Red);

                MessageBox.Show($"【升级失败】{ex.Message}");
            }
            finally
            {
                if(Directory.Exists(tempDir))
                    Directory.Delete(tempDir, true);
            }

            progressBar.Value = 100;
            progressBar.IsIndeterminate = false;
        }

        private void Select(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new()
            {
                Filter = "Zip files (*.zip)|*.zip"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                var sourceZipFile = openFileDialog.FileName;
                fileFullName.Text = sourceZipFile;
            }
        }

        private void Refresh(object sender, RoutedEventArgs e)
        {
            fileFullName.Text = string.Empty;
            progressBar.Value = 0;
        }
    }
}
