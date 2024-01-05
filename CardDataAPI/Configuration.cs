using CommonLibrary;
using DataAccess;
using TCG_Scraper;

namespace CardDataAPI
{
    public class Configuration
    {
        private ConnectionSettings? _connectionSettings;
        private ScraperSettings? _scraperSettings;

        public Configuration(IConfiguration config)
        {
            Config = config;
        }

        public IConfiguration Config { get; }
        public ConnectionSettings ConnectionSettings => _connectionSettings ??= GetConnectionSettings();
        public ScraperSettings ScraperSettings => _scraperSettings ??= GetScraperSettings();

        private ConnectionSettings GetConnectionSettings()
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

        private ScraperSettings GetScraperSettings()
        {
            var settings = Config.GetSection("ScraperSettings");
            return new ScraperSettings()
            {
                SaveJsonPath = settings.GetSection("SaveJsonPath").Value.AsString(),
                SkipScrape = settings.GetSection("SkipScrape").Value.AsBool(),
                DelayBetweenRequests = TimeSpan.FromMilliseconds(settings.GetSection("DelayBetweenRequests").Value.AsInt()),
                CardsPerRequest = settings.GetSection("CardsPerRequest").Value.AsInt(),
                MaxCardsPerSet = settings.GetSection("MaxCardsPerSet").Value.AsInt(),
            };
        }
    }
}
