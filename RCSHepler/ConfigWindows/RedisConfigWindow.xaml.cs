using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RCSHepler.Services;
using RCSHepler.ViewModels;
using System.IO;
using System.Net;
using System.Windows;
using Formatting = Newtonsoft.Json.Formatting;

namespace RCSHepler.ConfigWindows
{
    /// <summary>
    /// WindowConfiguration.xaml 的交互逻辑
    /// </summary>
    public partial class RedisConfigWindow : Window
    {
        public RedisConfigViewModel RedisConfiguration { get; set; } = new();

        public string ConnectionStatus { get; set; } = string.Empty;

        private readonly Dictionary<string, object> _cache = new();

        public RedisConfigWindow(string? status)
        {
            InitializeComponent();

            ConnectionStatus = status!;

            try
            {
                var (ipEndpoint, password) = RedisService.GetConfigurationOption();

                RedisConfiguration.IPEndpoint = ipEndpoint.ToString();
                RedisConfiguration.Password = password;

                _cache.Add(nameof(RedisConfiguration.IPEndpoint), ipEndpoint);
                _cache.Add(nameof(RedisConfiguration.Password), password);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                DataContext = RedisConfiguration;
            }
        }

        private void Connect(object sender, RoutedEventArgs e)
        {
            try
            {
                var endpoint = IPEndPoint.Parse(RedisConfiguration.IPEndpoint!);

                if (RedisService.TryConnect((endpoint, RedisConfiguration.Password!), out string message))
                {
                    ConnectionStatus = message;
                }
                else
                {
                    ConnectionStatus = "连接失败";
                }

                MessageBox.Show(message);
            }
            catch (FormatException ex) 
            {
                MessageBox.Show($"【连接失败，IPEndpoint解析错误】{ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"【连接失败】{ex.Message}");
            }
        }

        private void Reset(object sender, RoutedEventArgs e)
        {
            if(_cache.TryGetValue(nameof(RedisConfiguration.IPEndpoint), out var endPoint))
                RedisConfiguration.IPEndpoint = endPoint.ToString();

            if (_cache.TryGetValue(nameof(RedisConfiguration.Password), out var password))
                RedisConfiguration.Password = password.ToString();
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            try
            {
                var json = RedisService.GetAgvSysManagerConfigJson(out string path);

                JObject obj = JObject.Parse(json);

                obj["RedisHosts"] = RedisConfiguration.IPEndpoint;
                obj["RedisPWD"] = RedisConfiguration.Password;

                var token = JToken.Parse(JsonConvert.SerializeObject(obj));

                File.WriteAllText(path, token.ToString(Formatting.Indented));

                var result = MessageBox.Show("保存成功\n【需要重启AgvSysManager服务让配置生效，是否立即重启】", "", MessageBoxButton.YesNo);

                if(result == MessageBoxResult.Yes)
                {
                    //var schedulingService = SMPage._schedulingServices
                    //    .FirstOrDefault(s => s.ServiceName == ConfigurationManager.AppSettings["SysManager"]);

                    //var service = WindowsLocalService.Stop(schedulingService.ServiceName!);

                    //schedulingService.ServiceStatus = service.Status.ToString();

                    //await Task.Delay(TimeSpan.FromSeconds(1));

                    //var service2 = WindowsLocalService.Start(schedulingService.ServiceName!);

                    //schedulingService.ServiceStatus = service2.Status.ToString();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
