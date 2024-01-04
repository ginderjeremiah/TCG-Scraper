using TCG_Scraper;
using TcgScraperTests.Mocks;

namespace TcgScraperTests
{
    [TestClass]
    public class TcgCardRequesterTests
    {
        [TestMethod]
        public async Task GetProductLine_InvalidProductLineName_ThrowsException()
        {
            var productLineName = "DoesNotExist";
            var logger = new TestLogger();
            var requester = new TcgCardRequester(logger);

            await Assert.ThrowsExceptionAsync<Exception>(async () => await requester.GetProductLine(p => p.ProductLineName == productLineName));
        }

        [TestMethod]
        public async Task GetProductLine_InvalidProductLineId_ThrowsException()
        {
            var productLineId = -1;
            var logger = new TestLogger();
            var requester = new TcgCardRequester(logger);

            await Assert.ThrowsExceptionAsync<Exception>(async () => await requester.GetProductLine(p => p.ProductLineId == productLineId));
        }

        [TestMethod]
        public async Task GetProductLine_ValidProductLineNameLoadsSuccessfully()
        {
            var productLineName = "Flesh and Blood TCG";
            var logger = new TestLogger();
            var requester = new TcgCardRequester(logger);

            var productLine = await requester.GetProductLine(p => p.ProductLineName == productLineName);

            Assert.IsTrue(productLine.ProductLineName == productLineName);
        }

        [TestMethod]
        public async Task GetProductLines_LoadsSuccessfully()
        {
            var logger = new TestLogger();
            var requester = new TcgCardRequester(logger);

            var productLines = await requester.GetProductLines();

            Assert.IsTrue(productLines is not null && productLines.Any());
        }

        [TestMethod]
        public async Task GetSets_InvalidProductLineId_ThrowsException()
        {
            var productLineId = -1;
            var logger = new TestLogger();
            var requester = new TcgCardRequester(logger);

            await Assert.ThrowsExceptionAsync<Exception>(async () => await requester.GetSets(productLineId));
        }

        [TestMethod]
        public async Task GetSets_ValidProductLineId_LoadsSuccessfully()
        {
            var productLineId = 62;
            var logger = new TestLogger();
            var requester = new TcgCardRequester(logger);

            var sets = await requester.GetSets(productLineId);

            Assert.IsTrue(sets is not null && sets.Any());
        }

        [TestMethod]
        public async Task GetTotalCardsForProductLine_InvalidProductLine_ReturnsAllCardCount()
        {
            var productLineUrlName = "DoesNotExist";
            var logger = new TestLogger();
            var requester = new TcgCardRequester(logger);

            var totalCards = await requester.GetTotalCardsForProductLine(productLineUrlName);

            Assert.IsTrue(totalCards > 300000);
        }

        [TestMethod]
        public async Task GetTotalCardsForProductLine_ValidProductLine_ReturnsCardCountForProductLine()
        {
            var productLineName = "Flesh and Blood TCG";
            var logger = new TestLogger();
            var requester = new TcgCardRequester(logger);
            var productLine = await requester.GetProductLine(p => p.ProductLineName == productLineName);
            var totalCards = await requester.GetTotalCardsForProductLine("");

            var cardsInProductLine = await requester.GetTotalCardsForProductLine(productLine.ProductLineUrlName);

            Assert.IsTrue(cardsInProductLine > 0 && cardsInProductLine != totalCards);
        }

        [TestMethod]
        public async Task RequestCardInfos_InvalidProductLineName_ReturnsSubsetOfAllCards()
        {
            var productLineName = "DoesNotExist";
            var numResults = 5;
            var logger = new TestLogger();
            var requester = new TcgCardRequester(logger);
            var totalCards = await requester.GetTotalCardsForProductLine("");

            var results = await requester.RequestCardInfos(productLineName, "", 0, numResults);

            Assert.IsTrue(results.Results.Count == numResults && results.TotalResults == totalCards);
        }

        [TestMethod]
        public async Task RequestCardInfos_InvalidSetSearchName_ReturnsSubsetOfAllCardsInProductLine()
        {
            var productLineName = "Flesh and Blood TCG";
            var setSearchName = "DoesNotExist";
            var numResults = 5;
            var logger = new TestLogger();
            var requester = new TcgCardRequester(logger);
            var productLine = await requester.GetProductLine(p => p.ProductLineName == productLineName);
            var totalCards = await requester.GetTotalCardsForProductLine(productLine.ProductLineUrlName);

            var results = await requester.RequestCardInfos(productLine.ProductLineUrlName, setSearchName, 0, numResults);

            Assert.IsTrue(results.Results.Count == numResults && results.TotalResults == totalCards);
        }

        [TestMethod]
        public async Task RequestCardInfos_ValidData_ReturnsDataSuccessfully()
        {
            var productLineName = "Flesh and Blood TCG";
            var numResults = 5;
            var logger = new TestLogger();
            var requester = new TcgCardRequester(logger);
            var productLine = await requester.GetProductLine(p => p.ProductLineName == productLineName);
            var totalCards = await requester.GetTotalCardsForProductLine(productLine.ProductLineUrlName);
            var set = (await requester.GetSets(productLine.ProductLineId)).First();
            var setSearchName = set.CleanSetName.ToLower().Replace(" ", "-");

            var results = await requester.RequestCardInfos(productLine.ProductLineUrlName, setSearchName, 0, numResults);

            Assert.IsTrue(results.Results.Count == numResults && results.TotalResults != totalCards);
        }
    }
}
