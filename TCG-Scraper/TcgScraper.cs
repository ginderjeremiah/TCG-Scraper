using ApiModels;
using CommonLibrary;
using DataAccess;
using System.Diagnostics;
using System.Text.Json;

namespace TCG_Scraper
{
    public class TcgScraper
    {
        private ProductLine? _productLine;
        private Timer? _timer;

        private ILogger Logger { get; set; }
        private IDataAccess DataAccess { get; set; }
        private TcgCardLoader CardLoader { get; set; }
        private TcgCardRequester CardRequester { get; set; }

        public int CardsPerRequest { get; set; } = 48;
        public string? SaveJsonPath { get; set; }
        public bool SkipScrape { get; set; } = false;
        public TimeSpan DelayBetweenRequests { get; set; } = TimeSpan.Zero;
        public int MaxCardsPerSet { get; set; } = int.MaxValue; //Mostly just for testing purposes

        public TcgScraper(IDataAccess? dataAccess = null, ILogger? logger = null)
        {
            DataAccess = dataAccess ?? new DataAccess.DataAccess(Configuration.ConnectionSettings);
            Logger = logger ?? new Logger();
            CardLoader = new(Logger, DataAccess);
            CardRequester = new TcgCardRequester(Logger);
        }

        public void ExecuteAtIntervals(TimeSpan timeToFirstRun, TimeSpan timeBetweenEachRun, string productLineName)
        {
            if (_timer is not null)
                throw new Exception("Timer already exists.");

            _timer = new Timer((object state) => LoadCardsByProductLine((string)state), productLineName, timeToFirstRun, timeBetweenEachRun);
        }

        public void ExecuteAtIntervals(TimeSpan timeToFirstRun, TimeSpan timeBetweenEachRun, int productLineId)
        {
            if (_timer is not null)
                throw new Exception("Timer already exists.");

            _timer = new Timer((object state) => LoadCardsByProductLine((int)state), productLineId, timeToFirstRun, timeBetweenEachRun);
        }

        public async Task LoadCardsByProductLine(string productLineName)
        {
            try
            {
                if (_productLine is null || _productLine.ProductLineName != productLineName)
                {
                    _productLine = await CardRequester.GetProductLine(p => p.ProductLineName == productLineName);
                }
                await LoadCardsByProductLine(_productLine);
            }
            catch (Exception e)
            {
                Logger.LogError(e);
                throw;
            }

        }
        public async Task LoadCardsByProductLine(int productLineId)
        {
            try
            {
                if (_productLine is null || _productLine.ProductLineId != productLineId)
                {
                    _productLine = await CardRequester.GetProductLine(p => p.ProductLineId == productLineId);
                }

                await LoadCardsByProductLine(_productLine);
            }
            catch (Exception e)
            {
                Logger.LogError(e);
                throw;
            }
        }

        private async Task LoadCardsByProductLine(ProductLine productLine)
        {
            Logger.Log($"Beginning loading cards for {productLine.ProductLineName}.");
            long startTime = Stopwatch.GetTimestamp();

            IEnumerable<CardInfo> cards;

            if (!SkipScrape || !File.Exists(SaveJsonPath))
            {
                var getSets = CardRequester.GetSets(productLine.ProductLineId);
                var getTotals = CardRequester.GetTotalCardsForProductLine(productLine.ProductLineUrlName);

                List<SetInfo> sets = await getSets;
                int totalCards = await getTotals;

                IEnumerable<List<CardInfo>> cardLists = new List<List<CardInfo>>();

                foreach (var set in sets)
                {
                    try
                    {
                        cardLists = cardLists.Concat(await ScrapeCardsInSet(productLine, set, totalCards));
                    }
                    catch (Exception e)
                    {
                        Logger.LogError(e.ToString());
                    }
                }

                cards = cardLists.SelectMany(lists => lists);
            }
            else
            {
                cards = JsonSerializer.Deserialize<List<CardInfo>>(File.OpenRead(SaveJsonPath))
                    ?? throw new Exception($"Failed to parse json file: {Path.GetFullPath(SaveJsonPath)}");
            }

            CardLoader.ImportAllCardData(cards, productLine.ProductLineId);

            var elapsedTime = Stopwatch.GetElapsedTime(startTime).TotalSeconds;
            Logger.Log($"Finished loading cards for {productLine.ProductLineName}. Elapsed time: {elapsedTime} seconds.");
        }

        public async Task<List<List<CardInfo>>> ScrapeCardsInSet(ProductLine productLine, SetInfo setInfo, int totalCardsInProductLine)
        {
            List<List<CardInfo>> cardLists = new();

            if (MaxCardsPerSet < 1)
                return cardLists;

            var setSearchName = setInfo.CleanSetName.ToLower().Replace(" ", "-");
            var results = await CardRequester.RequestCardInfos(productLine.ProductLineUrlName, setSearchName, 0, Math.Min(CardsPerRequest, MaxCardsPerSet));
            var setTotal = results.TotalResults;
            var maxPull = Math.Min(setTotal, MaxCardsPerSet);

            if (setTotal == totalCardsInProductLine)
                throw new Exception($"{setSearchName} returned the total numbers of cards in the product lines");

            cardLists.Add(results.Results);

            if (DelayBetweenRequests > TimeSpan.Zero)
                await Task.Delay(DelayBetweenRequests);

            for (int i = CardsPerRequest; i < maxPull; i += CardsPerRequest)
            {
                var cardsToPull = Math.Min(CardsPerRequest, maxPull - i - 1);
                results = await CardRequester.RequestCardInfos(productLine.ProductLineUrlName, setSearchName, i, cardsToPull);
                cardLists.Add(results.Results);

                if (DelayBetweenRequests > TimeSpan.Zero)
                    await Task.Delay(DelayBetweenRequests);
            }

            return cardLists;
        }
    }
}
