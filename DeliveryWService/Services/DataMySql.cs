using System;
using System.Collections.Generic;
using System.Configuration;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace DeliveryWService.Services
{
    public class DBConnection
    {
        private DBConnection()
        {
        }

        public string Password { get; set; }
        private MySqlConnection connection = null;
        public MySqlConnection Connection
        {
            get { return connection; }
        }

        private static DBConnection _instance = null;
        public static DBConnection Instance()
        {
            if (_instance == null)
                _instance = new DBConnection();
            return _instance;
        }

        public bool IsConnect()
        {
            if (Connection == null)
            {
                if (String.IsNullOrEmpty(ConfigurationManager.AppSettings["mySqlDBName"]))
                    return false;
                string connstring = string.Format("Server={0}; Port = {4}; database={1}; UID={2}; password={3}; persistsecurityinfo=True; SslMode=none", ConfigurationManager.AppSettings["mySqlDBServer"], ConfigurationManager.AppSettings["mySqlDBName"], ConfigurationManager.AppSettings["mySqlDBuser"], ConfigurationManager.AppSettings["mySqlDBPassword"], ConfigurationManager.AppSettings["mySqlDBPort"]);
                connection = new MySqlConnection(connstring);
                connection.Open();
            }
            else if(Connection.State != System.Data.ConnectionState.Open)
            {
                if (String.IsNullOrEmpty(ConfigurationManager.AppSettings["mySqlDBName"]))
                    return false;
                string connstring = string.Format("Server={0}; Port = {4}; database={1}; UID={2}; password={3}; persistsecurityinfo=True; SslMode=none", ConfigurationManager.AppSettings["mySqlDBServer"], ConfigurationManager.AppSettings["mySqlDBName"], ConfigurationManager.AppSettings["mySqlDBuser"], ConfigurationManager.AppSettings["mySqlDBPassword"], ConfigurationManager.AppSettings["mySqlDBPort"]);
                connection = new MySqlConnection(connstring);
                connection.Open();
            }
            return true;
        }

        public void Close()
        {
            connection.Close();
        }
    }
}