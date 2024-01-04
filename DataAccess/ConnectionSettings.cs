using Npgsql;

namespace DataAccess
{
    public class ConnectionSettings
    {
        public int Port { get; set; }
        public string Host { get; set; }
        public string Database { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public string GetConnectionString()
        {
            var connBuilder = new NpgsqlConnectionStringBuilder
            {
                Port = Port,
                Host = Host,
                Database = Database,
                Username = Username,
                Password = Password
            };

            return connBuilder.ConnectionString;
        }
    }
}
