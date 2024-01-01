using Npgsql;

namespace DataAccess
{
    public static class Configuration
    {
        //private static IConfiguration? _configuration;
        private static string? _connectionString;

        //private static IConfiguration Configuration
        //{
        //    get
        //    {
        //        _configuration ??= new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        //        return _configuration;
        //    }
        //}

        public static string ConnectionString
        {
            get => _connectionString ?? GetConnectionString();
        }

        private static string GetConnectionString()
        {
            var connBuilder = new NpgsqlConnectionStringBuilder
            {
                Port = 5432,
                Username = "card_reader_uploader",
                Password = "TCG$craper32",
                Host = "localhost",
                Database = "CardData"
            };

            _connectionString = connBuilder.ToString();
            return _connectionString;
        }
    }
}
