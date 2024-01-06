using DataAccess.SqlModels;
using System.Text.Json;
using Tests.Mocks;

namespace Tests
{
    [TestClass]
    public class CardsControllerTests
    {
        [TestMethod]
        public async Task Cards_NoParameters_ReturnsData()
        {
            await using var app = new ApiAppFactory();
            using var client = app.CreateClient();
            var cards = MockCardsImport.InitTestCards();
            app.Repositories.Cards = new MockCardsImport();
            app.Repositories.Cards.ImportCards(cards);

            var response = await client.GetAsync("/api/Cards");

            var data = response.Deserialize<List<Card>>();

            Assert.IsTrue(response.IsSuccessStatusCode);
            Assert.IsNotNull(data);
            Assert.AreEqual(cards.Count, data.Count);
            Assert.IsTrue(data.All(c1 => cards.Any(c2 => c1.ProductId == c2.ProductId)));
        }

        [TestMethod]
        public async Task Cards_SkipAndLimit_ReturnsCorrectData()
        {
            var skip = 1;
            var limit = 2;
            await using var app = new ApiAppFactory();
            using var client = app.CreateClient();
            var cards = MockCardsImport.InitTestCards();
            app.Repositories.Cards = new MockCardsImport();
            app.Repositories.Cards.ImportCards(cards);

            var response = await client.GetAsync($"/api/Cards?offset={skip}&limit={limit}");

            var data = response.Deserialize<List<Card>>();

            var cardsSubset = cards.Skip(skip).Take(limit);

            Assert.IsTrue(response.IsSuccessStatusCode);
            Assert.IsNotNull(data);
            Assert.AreEqual(cardsSubset.Count(), data.Count);
            Assert.IsTrue(data.All(c1 => cardsSubset.Any(c2 => c1.ProductId == c2.ProductId)));
        }
    }
}
