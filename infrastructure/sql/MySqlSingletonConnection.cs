using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MySql.Data.MySqlClient;
using System;

namespace sgi_app.infrastructure.sql
{
    public class MySqlSingletonConnection
    {
        private static MySqlSingletonConnection _instance;
        private MySqlConnection _connection;

        private MySqlSingletonConnection()
        {
            string connectionString = "Server=localhost;Port=3306;Database=sgi-db;User=sgiapp;Password=kodigo777;SslMode=none;AllowPublicKeyRetrieval=true;";
            
            _connection = new MySqlConnection(connectionString);
        }

        public static MySqlSingletonConnection Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MySqlSingletonConnection();
                }
                return _instance;
            }
        }

        public MySqlConnection GetConnection()
        {
            return _connection;
        }

        public bool TestConnection()
        {
            try
            {
                _connection.Open();
                Console.WriteLine("✅ Connection to the database was successful.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Connection to the database failed: {ex.Message}");
                return false;
            }
            finally
            {
                if (_connection.State == System.Data.ConnectionState.Open)
                {
                    _connection.Close();
                }
            }
        }

        public static void TestDirectConnection()
        {
            string cs = "Server=localhost;Port=3306;Database=sgi-db;User=root;Password=kodigo777;SslMode=none;AllowPublicKeyRetrieval=true;";
            using (var connection = new MySqlConnection(cs))
            {
                try
                {
                    connection.Open();
                    Console.WriteLine("Conexión directa exitosa!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error en conexión directa: {ex.Message}");
                }
            }
        }
    }
}