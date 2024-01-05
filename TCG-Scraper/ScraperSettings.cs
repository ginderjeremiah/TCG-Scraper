namespace TCG_Scraper
{
    public class ScraperSettings
    {
        public string? SaveJsonPath { get; set; }
        public bool SkipScrape { get; set; }
        public TimeSpan DelayBetweenRequests { get; set; }
        public int CardsPerRequest { get; set; }
        public int MaxCardsPerSet { get; set; }
    }
}
