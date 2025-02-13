using Google.Protobuf.WellKnownTypes;
using Microsoft.Win32;
using Org.BouncyCastle.Tls;
using Org.BouncyCastle.Utilities.IO;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceProcess;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace RCSHepler.Services
{
    internal static class RedisService
    {
        /// <summary>
        /// 获取Redis配置文件
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetAgvSysManagerConfigJson(out string path)
        {
            var value = RegisterService.GetImagePath("SysManager");

            var folderPath = Path.GetDirectoryName(value) ?? string.Empty;

            var jsonPath = Path.Combine(folderPath, "appsettings.json");

            path = jsonPath;

            return File.ReadAllText(jsonPath);
        }

        /// <summary>
        /// 获取Redis配置信息
        /// </summary>
        /// <returns></returns>
        public static (IPEndPoint endpoint, string password) GetConfigurationOption()
        {
            string json = GetAgvSysManagerConfigJson(out string _);

            using JsonDocument jsonDocument = JsonDocument.Parse(json, new() { CommentHandling = JsonCommentHandling.Skip });

            string ipEndPoint = jsonDocument.RootElement.TryGetProperty("RedisHosts", out JsonElement host)
                ? host.GetString() ?? string.Empty : string.Empty;

            string password = jsonDocument.RootElement.TryGetProperty("RedisPWD", out JsonElement pwd)
                ? pwd.GetString() ?? string.Empty : string.Empty;

            IPEndPoint.TryParse(ipEndPoint, out IPEndPoint? re);

            return (IPEndPoint.Parse(ipEndPoint), password);
        }

        /// <summary>
        /// 连接尝试
        /// </summary>
        /// <param name="option"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static bool TryConnect((IPEndPoint endpoint, string password) option, out string message)
        {
            bool result = false;
            message = string.Empty;

            try
            {
                ConfigurationOptions options = new()
                {
                    EndPoints = { option.endpoint },
                    Password = option.password
                };

                using ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(options);

                if (redis.IsConnected)
                {
                    result = true;
                    message = "已连接";
                }
            }
            catch (Exception ex)
            {
                message = $"【Redis连接失败】{ex.Message}";
            }

            return result;
        }

        /// <summary>
        /// 创建Redis客户端实例
        /// </summary>
        /// <returns></returns>
        public static ConnectionMultiplexer CreateRedis()
        {
            (IPEndPoint endpoint, string password) = GetConfigurationOption();

            ConfigurationOptions options = new()
            {
                EndPoints = { endpoint },
                Password = password
            };

            ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(options);

            return redis;
        }

        /// <summary>
        /// 读取值
        /// </summary>
        /// <param name="redis"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string? GetValue(this ConnectionMultiplexer redis, string key)
        {
            var dataBase = redis.GetDatabase();

            var value = dataBase.StringGet(key);

            return value;
        }

        /// <summary>
        /// 写入值
        /// </summary>
        /// <param name="redis"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void SetValue(this ConnectionMultiplexer redis, string key, string? value) 
        {
            var dataBase = redis.GetDatabase();

            dataBase.StringSet(key, value);
        }
    }
}
