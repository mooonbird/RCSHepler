using Microsoft.Win32;
using RCSHepler.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    /// AuthorizationInformation.xaml 的交互逻辑
    /// </summary>
    public partial class AuthorizationInformationPage : Page
    {
        public MainWindow MainWindow { get; set; } = null!;

        public AuthorizationInformationPage()
        {
            InitializeComponent();
        }

        private void Improt(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new()
            {
                Filter = "Key files (*.key)|*.key"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    switch (LicenseService.VerifyLicense(openFileDialog.FileName))
                    {
                        case true:
                            MainWindow.SoftwareInfoViewModel.AuthorizationStatus = "本机授权成功";
                            break;
                        case false:
                            MainWindow.SoftwareInfoViewModel.AuthorizationStatus = "本机授权失败";
                            break;
                        default:
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    MainWindow.SoftwareInfoViewModel.AuthorizationStatus = "本机授权失败";
                }
            }
        }

        private void Export(object sender, RoutedEventArgs e)
        {
            string info = string.Empty;

            foreach (var item in WMIService.GetCertificateAuthorityInfo())
            {
                info += $"{item.Key}={item.Value}\n";
            }

            SaveFileDialog saveFileDialog = new()
            {
                FileName = "ComputerInfo.key",
                Filter = "Key files (*.key)|*.key"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                MessageBox.Show(saveFileDialog.FileName);

                File.WriteAllText(saveFileDialog.FileName, info);
            }

        }
    }
}
