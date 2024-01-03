using ApiModels;
using DataAccess.SqlModels;
using System.Text.Json;
using TCG_Scraper;

namespace TcgScraperTests
{
    [TestClass]
    public class TcgCardLoaderTests
    {
        [TestMethod]
        public void ImportAllCardData_LoadsSingleCardDataSuccessfully()
        {
            var productLineId = 62;
            var logger = new TestLogger();
            var dataAccess = new TestDataAccess();
            var cards = new TestCardsImport();
            var customAttributes = new TestCustomAttributesImport();
            var customAttributesValues = new TestCustomAttributesValuesImport();
            dataAccess.Cards = cards;
            dataAccess.CustomAttributes = customAttributes;
            dataAccess.CustomAttributesValues = customAttributesValues;
            var loader = new TcgCardLoader(logger, dataAccess);
            var cardData = new List<CardInfo> { new CardInfo()
            {
                ProductId = 1.0f,
                CustomAttributes = new Dictionary<string, JsonElement> { { "Test", default } }
            } };

            //var customAttributes = new List<CustomAttribute>() { new CustomAttribute() };
            //var customAttributesValues = new List<CustomAttributesValue>() { new CustomAttributesValue() };

            loader.ImportAllCardData(cardData, productLineId);
            Assert.IsTrue(cards.DataLoaded is not null
                && cards.DataLoaded.FirstOrDefault()?.ProductId == 1);

            Assert.IsTrue(customAttributes.DataLoaded is not null
                && customAttributes.DataLoaded.FirstOrDefault()?.ProductLineId == 62
                && customAttributes.DataLoaded.FirstOrDefault()?.Name == "Test");

            Assert.IsTrue(customAttributesValues.DataLoaded is not null
                && customAttributesValues.DataLoaded.FirstOrDefault()?.ProductId == 1);
        }
    }
    internal class TestCustomAttributesImport : TestCustomAttributes
    {
        public Task? AwaitableTask { get; set; }
        public IEnumerable<CustomAttribute>? DataLoaded { get; set; }
        public override void ImportCustomAttributes(IEnumerable<CustomAttribute> atts)
        {
            DataLoaded = atts.Select((att, i) =>
            {
                att.CustomAttributeId = i;
                return att;
            });
            AwaitableTask?.Start();
        }

        public override List<CustomAttribute> GetAttributesByProductLine(int productLineId)
        {
            return DataLoaded?.ToList() ?? new List<CustomAttribute>();
        }
    }

    internal class TestCustomAttributesValuesImport : TestCustomAttributesValues
    {
        public Task? AwaitableTask { get; set; }
        public IEnumerable<CustomAttributesValue>? DataLoaded { get; set; }
        public override void ImportCustomAttributesValues(IEnumerable<CustomAttributesValue> atts)
        {
            DataLoaded = atts;
            AwaitableTask?.Start();
        }
    }
}
