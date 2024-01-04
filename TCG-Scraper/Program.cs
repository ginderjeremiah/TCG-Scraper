using TCG_Scraper;

string productLineName = "Flesh and Blood TCG";
string saveDataFileName = "cardData_fb.json";
int cardsPerRequest = 48;
bool skipScrape = false;

var scraper = new TcgScraper()
{
    SaveJsonPath = saveDataFileName,
    CardsPerRequest = cardsPerRequest,
    SkipScrape = skipScrape,
};

scraper.ExecuteAtIntervals(TimeSpan.Zero, TimeSpan.FromMinutes(2), productLineName);

await Task.Delay(TimeSpan.FromMinutes(5));