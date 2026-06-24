
using System;
using System.Configuration;
using MySql.Data.MySqlClient;

namespace TubesKPL
{
    public static class DatabaseConnection
    {
        public static string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(GetConnectionString());
        }
    }
}
