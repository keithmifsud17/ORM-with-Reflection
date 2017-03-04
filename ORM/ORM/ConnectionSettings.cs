using System;
using System.Collections.Generic;
using System.Linq;

namespace ORM
{
    public class ConnectionSettings
    {
        private static ConnectionSettings _Instance = null;
        public static ConnectionSettings Instance => _Instance ?? (_Instance = new ConnectionSettings());

        private Dictionary<string, string> ConnectionStrings;

        public ConnectionSettings()
        {
            ConnectionStrings = new Dictionary<string, string>();
        }

        public void RegisterConnectionString(string Key, string ConnectionString)
        {
            ConnectionStrings.Add(Key, ConnectionString);
        }

        public string GetConnectionString()
        {
            if (ConnectionStrings.Count > 0)
            {
                return ConnectionStrings.FirstOrDefault().Value;
            }
            throw new InvalidOperationException("No Connection string has been registered");
        }

        public string GetConnectionString(string Key)
        {
            return ConnectionStrings[Key];
        }
    }
}
