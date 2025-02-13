using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RCSHepler.Services
{
    public class MySQLService
    {
        public static bool TryConnect(out string errorMessage)
        {
            bool result = false;
            errorMessage = string.Empty;

            try
            {
                using MySqlConnection conn = new(ConfigurationManager.AppSettings["MYSQL"]);

                conn.Open();

                if (conn.State == ConnectionState.Open)
                {
                    result = true;
                    errorMessage = "已连接";
                }
            }
            catch (Exception ex)
            {
                errorMessage = $"【MySQL连接失败】{ex.Message}";
            }

            return result;
        }

        public static object? GetScalar(string value, string conditionKey, string conditionValue)
        {
            object? result = null;

            using MySqlConnection conn = new(ConfigurationManager.AppSettings["MYSQL"]);

            conn.Open();

            var sql = $"SELECT {value} FROM CUS_AGVHEARTCONFIG WHERE {conditionKey} = '{conditionValue}'";

            using MySqlCommand cmd = new(sql, conn);

            object scalar = cmd.ExecuteScalar();

            if (scalar != null)
            {
                result = scalar;
            }

            return result;
        }
    }
}
