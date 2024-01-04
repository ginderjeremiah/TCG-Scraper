using CommonLibrary;
using DataAccess;
using Microsoft.Extensions.Configuration;

namespace TCG_Scraper
{
    public static class Configuration
    {
        private static IConfiguration? _configuration;
        private static ConnectionSettings? _connectionSettings;

        public static IConfiguration Config => _configuration ??= new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

        public static ConnectionSettings ConnectionSettings => _connectionSettings ??= GetConnectionSettings();

        private static ConnectionSettings GetConnectionSettings()
        {
            var settings = Config.GetSection("ConnectionSettings");
            return new ConnectionSettings()
            {
                Port = settings.GetSection("Port").Value.AsInt(),
                Host = settings.GetSection("Host").Value.AsString(),
                Database = settings.GetSection("Database").Value.AsString(),
                Username = settings.GetSection("Username").Value.AsString(),
                Password = settings.GetSection("Password").Value.AsString()
            };
        }
    }
}
