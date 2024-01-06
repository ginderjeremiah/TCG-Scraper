using Tests.Mocks;

namespace Tests
{
    [TestClass]
    public class ScraperControllerTests
    {
        [TestMethod]
        public async Task RefreshProductLine_InvalidProductLine_ReturnsError()
        {
            var productLineId = -1;
            await using var app = new ApiAppFactory();
            using var client = app.CreateClient();
            app.Repositories.Cards = new MockCardsImport();
            app.Repositories.CustomAttributes = new MockCustomAttributesImport();
            app.Repositories.CustomAttributesValues = new MockCustomAttributesValues();

            var response = await client.GetAsync($"/api/Scraper/RefreshProductLine?productLine={productLineId}");

            var data = app.Repositories.Cards.GetCards();

            Assert.IsFalse(response.IsSuccessStatusCode);
            Assert.IsFalse(data.Any());
        }

        [TestMethod]
        public async Task RefreshProductLine_ValidProductLine_LoadsData()
        {
            var productLineId = 62;
            await using var app = new ApiAppFactory();
            using var client = app.CreateClient();
            app.Repositories.Cards = new MockCardsImport();
            app.Repositories.CustomAttributes = new MockCustomAttributesImport();
            app.Repositories.CustomAttributesValues = new MockCustomAttributesValues();

            var response = await client.GetAsync($"/api/Scraper/RefreshProductLine?productLine={productLineId}");

            var data = app.Repositories.Cards.GetCards();

            Console.WriteLine(response.StatusCode);

            Assert.IsTrue(response.IsSuccessStatusCode);
            Assert.IsTrue(data.Any());
        }
    }
}
