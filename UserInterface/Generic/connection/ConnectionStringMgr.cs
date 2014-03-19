using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Configuration;

namespace AngularUI.Generic.connection
{
    public class ConnectionStringMgr
    {
        public static IList<ConnectionStringData> GetAllConnectionStrings()
        {
            IList<ConnectionStringData> list=new List<ConnectionStringData>();
            foreach (ConnectionStringSettings connectionString in ConfigurationManager.ConnectionStrings)
            {
                var item = new ConnectionStringData
                    {
                        Name = connectionString.Name,
                        ProviderName = connectionString.ProviderName,
                        ConnectionString = connectionString.ConnectionString
                    };
                try
                {
                    CreateParamsFromConnectionString(item);
                }
                catch (Exception)
                {
                    continue;
                }
                list.Add(item);
            }

            return list;
        }

        public static ConnectionStringData Save(ConnectionStringData connection)
        {
            if (connection == null)
                return null;
            
            //if (string.IsNullOrEmpty(connection.ConnectionString))
                connection.ConnectionString = CreateConnectionStringFromParams(connection);


            var config = WebConfigurationManager.OpenWebConfiguration("~");


            var settings = new ConnectionStringSettings
            {
                Name = connection.Name,
                ProviderName = connection.ProviderName,
                ConnectionString = connection.ConnectionString
            };


            var list = GetAllConnectionStrings();
            if (list.Any(x => x.Name == connection.Name))
                config.ConnectionStrings.ConnectionStrings.Remove(connection.Name);
            
            config.ConnectionStrings.ConnectionStrings.Add(settings);
            config.Save(ConfigurationSaveMode.Modified);

            return connection;
        }

        public static bool CheckConnection(ConnectionStringData connection)
        {
            if (connection == null)
                return false;
            connection.ConnectionString = CreateConnectionStringFromParams(connection);

            return CreateConnection(connection.ConnectionString);
        }

        public static IList<string> GetSectionsNames()
        {
            var config = WebConfigurationManager.OpenWebConfiguration("~");

            return (from ConfigurationSection cs in config.Sections select cs.SectionInformation.SectionName).ToList();
        }

        private static string CreateConnectionStringFromParams(ConnectionStringData data)
        {
            var connectionstring = "Data Source=" + data.DataSource + ";" +
                                   "Initial Catalog=" + data.InitialCataloge + ";" +
                                   "Persist Security Info=" + data.PersistSecurityInfo + ";" +
                                   "User ID=" + data.UserId + ";" +
                                   "Password=" + data.Password;
            return connectionstring;
        }

        private static void CreateParamsFromConnectionString(ConnectionStringData data)
        {
            if(data.ConnectionString==string.Empty)
                return;
            var paramarray = data.ConnectionString.Split(';');
            data.DataSource = paramarray[0].Split('=')[1];
            data.InitialCataloge = paramarray[1].Split('=')[1];
            data.PersistSecurityInfo = paramarray[2].Split('=')[1];
            data.UserId = paramarray[3].Split('=')[1];
            data.Password = paramarray[4].Split('=')[1];
        }

        private static bool CreateConnection(string connectionString)
        {
            bool isValidConnectionString;
            using (var con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    isValidConnectionString = con.State == ConnectionState.Open;
                }
                catch (Exception)
                {
                    isValidConnectionString = false;
                }
            }
            return isValidConnectionString;
        }
    }

    public class ConnectionStringData
    {
        public string Name { get; set; }

        public string DataSource { get; set; }

        public string InitialCataloge { get; set; }

        public string ProviderName { get; set; }

        public string PersistSecurityInfo { get; set; }

        public string UserId { get; set; }

        public string Password { get; set; }

        public string ConnectionString { get; set; }
    }
}