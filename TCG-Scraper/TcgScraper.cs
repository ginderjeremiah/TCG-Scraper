﻿using ApiModels;
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

        private IApiLogger Logger { get; set; }
        private IRepositoryManager DataAccess { get; set; }
        private TcgCardLoader CardLoader { get; set; }
        private TcgCardRequester CardRequester { get; set; }
        private ScraperSettings Settings { get; set; }

        public int CardsPerRequest
        {
            get => Settings.CardsPerRequest;
            set => Settings.CardsPerRequest = value;
        }

        public string? SaveJsonPath
        {
            get => Settings.SaveJsonPath;
            set => Settings.SaveJsonPath = value;
        }
        public bool SkipScrape
        {
            get => Settings.SkipScrape;
            set => Settings.SkipScrape = value;
        }
        public TimeSpan DelayBetweenRequests
        {
            get => Settings.DelayBetweenRequests;
            set => Settings.DelayBetweenRequests = value;
        }
        public int MaxCardsPerSet
        {
            get => Settings.MaxCardsPerSet;
            set => Settings.MaxCardsPerSet = value;
        }

        public TcgScraper(IRepositoryManager dataAccess, IApiLogger logger, ScraperSettings? settings = null)
        {
            DataAccess = dataAccess;
            Logger = logger;
            CardLoader = new(Logger, DataAccess);
            CardRequester = new(Logger);
            Settings = settings ?? new ScraperSettings
            {
                CardsPerRequest = 48,
                SaveJsonPath = null,
                SkipScrape = false,
                DelayBetweenRequests = TimeSpan.Zero,
                MaxCardsPerSet = int.MaxValue,
            };
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
                    _productLine = await LoadAndGetProductLine(p => p.ProductLineName == productLineName);
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
                    _productLine = await LoadAndGetProductLine(p => p.ProductLineId == productLineId);
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

            bool needScrape = !SkipScrape || !File.Exists(SaveJsonPath);

            if (needScrape)
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

            if (needScrape && !string.IsNullOrWhiteSpace(SaveJsonPath))
                File.WriteAllText(SaveJsonPath, JsonSerializer.Serialize(cards));

            CardLoader.ImportAllCardData(cards, productLine.ProductLineId);

            var elapsedTime = Stopwatch.GetElapsedTime(startTime).TotalSeconds;
            Logger.Log($"Finished loading cards for {productLine.ProductLineName}. Elapsed time: {elapsedTime} seconds.");
        }

        public async Task<List<List<CardInfo>>> ScrapeCardsInSet(ProductLine productLine, SetInfo setInfo, int totalCardsInProductLine = -1)
        {
            List<List<CardInfo>> cardLists = new();

            if (MaxCardsPerSet < 1)
                return cardLists;

            if (totalCardsInProductLine < 0)
                totalCardsInProductLine = await CardRequester.GetTotalCardsForProductLine(productLine.ProductLineUrlName);

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

        private async Task<ProductLine> LoadAndGetProductLine(Func<ProductLine, bool> validator)
        {
            var productLines = await CardRequester.GetProductLines();
            CardLoader.ImportProductLines(productLines);
            return productLines.FirstOrDefault(validator)
                ?? throw new Exception("Product Line not found.");
        }
    }
}
