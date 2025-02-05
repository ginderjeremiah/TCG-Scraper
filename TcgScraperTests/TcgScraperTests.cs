using ApiModels;
using TCG_Scraper;
using Tests.Mocks;

namespace Tests
{
    [TestClass]
    public class TcgScraperTests
    {
        [TestMethod]
        public async Task ExecuteAtIntervals_InvalidProductLineName_LogsError()
        {
            var productLineName = "DoesNotExist";
            var logger = new MockLogger();
            var dataAccess = new MockRepositoryManager();
            var scraper = new TcgScraper(dataAccess, logger);

            scraper.ExecuteAtIntervals(TimeSpan.Zero, TimeSpan.FromMinutes(100), productLineName);

            var errorLog = logger.AwaitNextErrorLog();
            var delay = Task.Delay(TimeSpan.FromSeconds(2));
            Assert.IsTrue((await Task.WhenAny(errorLog, delay)) == errorLog
                && await errorLog == "Product Line not found.");
        }

        [TestMethod]
        public async Task ExecuteAtIntervals_InvalidProductLineId_LogsError()
        {
            var productLineId = -1;
            var logger = new MockLogger();
            var dataAccess = new MockRepositoryManager();
            var scraper = new TcgScraper(dataAccess, logger);

            scraper.ExecuteAtIntervals(TimeSpan.Zero, TimeSpan.FromMinutes(100), productLineId);

            var errorLog = logger.AwaitNextErrorLog();
            var delay = Task.Delay(TimeSpan.FromSeconds(2));
            Assert.IsTrue((await Task.WhenAny(errorLog, delay)) == errorLog
                && await errorLog == "Product Line not found.");
        }

        [TestMethod]
        public async Task ExecuteAtIntervals_ValidProductLineName_LoadsData()
        {
            string productLineName = "Flesh and Blood TCG";
            var logger = new MockLogger();
            var dataLoaded = false;
            var dataAccess = new MockRepositoryManager();
            var testCards = new MockCardsImport
            {
                AwaitableTask = new Task(() => dataLoaded = true)
            };
            var testProductLines = new MockProductLinesImport();
            dataAccess.ProductLines = testProductLines;
            dataAccess.Cards = testCards;
            var scraper = new TcgScraper(dataAccess, logger)
            {
                MaxCardsPerSet = 10,
                CardsPerRequest = 10,
            };

            scraper.ExecuteAtIntervals(TimeSpan.Zero, TimeSpan.FromMinutes(100), productLineName);

            await Task.WhenAny(testCards.AwaitableTask, Task.Delay(TimeSpan.FromSeconds(20)));
            Assert.IsTrue(dataLoaded && testCards.DataLoaded is not null && testCards.DataLoaded.Any());
            Assert.IsTrue(testProductLines.DataLoaded is not null && testProductLines.DataLoaded.Any());
        }

        [TestMethod]
        public async Task ExecuteAtIntervals_RunsMoreThanOnce()
        {
            string productLineName = "DoesNotExist";
            var logger = new MockLogger();
            var dataAccess = new MockRepositoryManager();
            var scraper = new TcgScraper(dataAccess, logger)
            {
                MaxCardsPerSet = 10,
                CardsPerRequest = 10,
            };

            scraper.ExecuteAtIntervals(TimeSpan.Zero, TimeSpan.FromSeconds(3), productLineName);
            var delay1 = Task.Delay(TimeSpan.FromSeconds(5));
            var firstWait = await Task.WhenAny(delay1, logger.AwaitNextErrorLog());
            Assert.IsTrue(firstWait != delay1);
            var delay2 = Task.Delay(TimeSpan.FromSeconds(5));
            var secondWait = await Task.WhenAny(delay2, logger.AwaitNextErrorLog());
            Assert.IsTrue(secondWait != delay2);
        }

        [TestMethod]
        public async Task LoadCardsByProductLine_InvalidProductLineName_ThrowException()
        {
            var productLineName = "DoesNotExist";
            var logger = new MockLogger();
            var dataAccess = new MockRepositoryManager();
            var scraper = new TcgScraper(dataAccess, logger);

            await Assert.ThrowsExceptionAsync<Exception>(async () => await scraper.LoadCardsByProductLine(productLineName));
        }

        [TestMethod]
        public async Task LoadCardsByProductLine_InvalidProductLineId_ThrowException()
        {
            var productLineId = -1;
            var logger = new MockLogger();
            var dataAccess = new MockRepositoryManager();
            var scraper = new TcgScraper(dataAccess, logger);

            await Assert.ThrowsExceptionAsync<Exception>(async () => await scraper.LoadCardsByProductLine(productLineId));
        }

        [TestMethod]
        public async Task LoadCardsByProductLine_ValidProductLineId_LoadsData()
        {
            var productLineId = 62;
            var logger = new MockLogger();
            var dataLoaded = false;
            var dataAccess = new MockRepositoryManager();
            var testCards = new MockCardsImport
            {
                AwaitableTask = new Task(() => dataLoaded = true)
            };
            var testProductLines = new MockProductLinesImport();
            dataAccess.ProductLines = testProductLines;
            dataAccess.Cards = testCards;
            var scraper = new TcgScraper(dataAccess, logger)
            {
                MaxCardsPerSet = 10,
                CardsPerRequest = 10,
            };

            scraper.LoadCardsByProductLine(productLineId);

            await Task.WhenAny(testCards.AwaitableTask, Task.Delay(TimeSpan.FromSeconds(20)));
            Assert.IsTrue(dataLoaded && testCards.DataLoaded is not null && testCards.DataLoaded.Any());
            Assert.IsTrue(testProductLines.DataLoaded is not null && testProductLines.DataLoaded.Any());
        }

        [TestMethod]
        public async Task ScrapeCardsInSet_SimulatedInvalidSearchParams_ThrowsException()
        {
            var productLine = new ProductLine()
            {
                ProductLineUrlName = "Magic"
            };
            var setInfo = new SetInfo()
            {
                CleanSetName = "wilds-of-eldraine"
            };
            var totalCardsInSet = 450;
            var logger = new MockLogger();
            var dataAccess = new MockRepositoryManager();
            var scraper = new TcgScraper(dataAccess, logger)
            {
                MaxCardsPerSet = 5,
                CardsPerRequest = 5,
            };

            await Assert.ThrowsExceptionAsync<Exception>(async () => await scraper.ScrapeCardsInSet(productLine, setInfo, totalCardsInSet));
        }

        [TestMethod]
        public async Task ScrapeCardsInSet_ValidInputs_ReturnsData()
        {
            var productLine = new ProductLine()
            {
                ProductLineUrlName = "Magic"
            };
            var setInfo = new SetInfo()
            {
                CleanSetName = "wilds-of-eldraine"
            };
            var logger = new MockLogger();
            var dataAccess = new MockRepositoryManager();
            var scraper = new TcgScraper(dataAccess, logger)
            {
                MaxCardsPerSet = 5,
                CardsPerRequest = 5,
            };

            var results = await scraper.ScrapeCardsInSet(productLine, setInfo);

            Assert.IsTrue(results.Count == 1 && results[0].Count == 5);
        }
    }
}