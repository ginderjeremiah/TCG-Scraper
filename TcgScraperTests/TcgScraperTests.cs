using ApiModels;
using DataAccess.SqlModels;
using TCG_Scraper;
using TcgScraperTests.Mocks;

namespace TcgScraperTests
{
    [TestClass]
    public class TcgScraperTests
    {
        [TestMethod]
        public async Task ExecuteAtIntervals_InvalidProductLineName_ThrowsException()
        {
            var productLineName = "DoesNotExist";
            var logger = new TestLogger();
            var dataAccess = new TestDataAccess();
            var scraper = new TcgScraper(dataAccess, logger);

            scraper.ExecuteAtIntervals(TimeSpan.Zero, TimeSpan.FromMinutes(100), productLineName);

            var errorLog = logger.AwaitNextErrorLog();
            var delay = Task.Delay(TimeSpan.FromSeconds(2));
            Assert.IsTrue((await Task.WhenAny(errorLog, delay)) == errorLog
                && await errorLog == "Product Line not found.");
        }

        [TestMethod]
        public async Task ExecuteAtIntervals_InvalidProductLineId_ThrowsException()
        {
            var productLineId = -1;
            var logger = new TestLogger();
            var dataAccess = new TestDataAccess();
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
            var logger = new TestLogger();
            var dataLoaded = false;
            var dataAccess = new TestDataAccess();
            var testCards = new TestCardsImport
            {
                AwaitableTask = new Task(() => dataLoaded = true)
            };
            dataAccess.Cards = testCards;
            var scraper = new TcgScraper(dataAccess, logger)
            {
                MaxCardsPerSet = 10,
                CardsPerRequest = 10,
            };

            scraper.ExecuteAtIntervals(TimeSpan.Zero, TimeSpan.FromMinutes(100), productLineName);

            await Task.WhenAny(testCards.AwaitableTask, Task.Delay(TimeSpan.FromSeconds(20)));
            Assert.IsTrue(dataLoaded && testCards.DataLoaded is not null && testCards.DataLoaded.Any());
        }

        [TestMethod]
        public async Task ExecuteAtIntervals_RunsMoreThanOnce()
        {
            string productLineName = "DoesNotExist";
            var logger = new TestLogger();
            var dataAccess = new TestDataAccess();
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
            var logger = new TestLogger();
            var dataAccess = new TestDataAccess();
            var scraper = new TcgScraper(dataAccess, logger);

            await Assert.ThrowsExceptionAsync<Exception>(async () => await scraper.LoadCardsByProductLine(productLineName));
        }

        [TestMethod]
        public async Task LoadCardsByProductLine_InvalidProductLineId_ThrowException()
        {
            var productLineId = -1;
            var logger = new TestLogger();
            var dataAccess = new TestDataAccess();
            var scraper = new TcgScraper(dataAccess, logger);

            await Assert.ThrowsExceptionAsync<Exception>(async () => await scraper.LoadCardsByProductLine(productLineId));
        }

        [TestMethod]
        public async Task LoadCardsByProductLine_ValidProductLineId_LoadsData()
        {
            var productLineId = 62;
            var logger = new TestLogger();
            var dataLoaded = false;
            var dataAccess = new TestDataAccess();
            var testCards = new TestCardsImport
            {
                AwaitableTask = new Task(() => dataLoaded = true)
            };
            dataAccess.Cards = testCards;
            var scraper = new TcgScraper(dataAccess, logger)
            {
                MaxCardsPerSet = 10,
                CardsPerRequest = 10,
            };

            scraper.LoadCardsByProductLine(productLineId);

            await Task.WhenAny(testCards.AwaitableTask, Task.Delay(TimeSpan.FromSeconds(20)));
            Assert.IsTrue(dataLoaded && testCards.DataLoaded is not null && testCards.DataLoaded.Any());
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
            var logger = new TestLogger();
            var dataAccess = new TestDataAccess();
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
            var totalCardsInSet = 5;
            var logger = new TestLogger();
            var dataAccess = new TestDataAccess();
            var scraper = new TcgScraper(dataAccess, logger)
            {
                MaxCardsPerSet = 5,
                CardsPerRequest = 5,
            };

            var result = await scraper.ScrapeCardsInSet(productLine, setInfo, totalCardsInSet);

            Assert.IsTrue(result.Count == 1 && result[0].Count == 5);
        }
    }

    internal class TestCardsImport : TestCards
    {
        public Task? AwaitableTask { get; set; }
        public IEnumerable<Card>? DataLoaded { get; set; }
        public override void ImportCards(IEnumerable<Card> cards)
        {
            DataLoaded = cards;
            AwaitableTask?.Start();
        }
    }
}