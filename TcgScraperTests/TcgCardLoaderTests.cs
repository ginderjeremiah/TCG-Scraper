using ApiModels;
using CommonLibrary;
using DataAccess.SqlModels;
using System.Text.Json;
using TCG_Scraper;
using TcgScraperTests.Mocks;

namespace TcgScraperTests
{
    [TestClass]
    public class TcgCardLoaderTests
    {
        private TestLogger _logger = new();
        private TestDataAccess _dataAccess = new();
        private TestCardsImport _cards = new();
        private TestCustomAttributesImport _customAttributes = new();
        private TestCustomAttributesValuesImport _customAttributesValues = new();

        private void InitTestVars()
        {
            _logger = new();
            _dataAccess = new();
            _cards = new();
            _customAttributes = new();
            _customAttributesValues = new();
            _dataAccess.Cards = _cards;
            _dataAccess.CustomAttributes = _customAttributes;
            _dataAccess.CustomAttributesValues = _customAttributesValues;
        }

        [TestMethod]
        public void ImportAllCardData_LoadsSingleCardDataSuccessfully()
        {
            InitTestVars();
            var productLineId = 62;
            var loader = new TcgCardLoader(_logger, _dataAccess);
            var cardData = InitTestCardInfos().Take(1);

            loader.ImportAllCardData(cardData, productLineId);
            Assert.IsTrue(_cards.DataLoaded is not null
                && _cards.DataLoaded.FirstOrDefault()?.ProductId == 1);

            Assert.IsTrue(_customAttributes.DataLoaded is not null
                && _customAttributes.DataLoaded.FirstOrDefault()?.ProductLineId == productLineId
                && _customAttributes.DataLoaded.FirstOrDefault()?.Name == cardData.First().CustomAttributes.First().Key);

            Assert.IsTrue(_customAttributesValues.DataLoaded is not null
                && _customAttributesValues.DataLoaded.FirstOrDefault()?.ProductId == cardData.First().ProductId.AsInt());
        }

        public void ImportAllCardData_LoadsCardDataSuccessfullyIgnoringDuplicates()
        {
            InitTestVars();
            var productLineId = 62;
            var loader = new TcgCardLoader(_logger, _dataAccess);
            var cardData = InitTestCardInfos();
            var distinctData = cardData.DistinctBy(c => c.ProductId);
            var allAtts = distinctData.SelectMany(c => c.CustomAttributes);

            loader.ImportAllCardData(cardData, productLineId);

            Assert.IsTrue(_cards.DataLoaded is not null
                && _cards.DataLoaded.Count() == distinctData.Count());

            Assert.IsTrue(_customAttributes.DataLoaded is not null
                && _customAttributes.DataLoaded.All(ca => ca.ProductLineId == productLineId)
                && _customAttributes.DataLoaded.Count() == allAtts.DistinctBy(kvp => kvp.Key).Count());

            Assert.IsTrue(_customAttributesValues.DataLoaded is not null
                && _customAttributesValues.DataLoaded.Count() == allAtts.Count());
        }

        [TestMethod]
        public void ImportCards_LoadsSingleCardSuccessfully()
        {
            InitTestVars();
            var loader = new TcgCardLoader(_logger, _dataAccess);
            var cardData = InitTestCardInfos().Take(1);

            loader.ImportCards(cardData);

            Assert.IsTrue(_cards.DataLoaded is not null
                && _cards.DataLoaded.FirstOrDefault()?.ProductId == cardData.First().ProductId.AsInt());
        }

        [TestMethod]
        public void ImportCards_LoadsMultipleCardsSuccessfully()
        {
            InitTestVars();
            var loader = new TcgCardLoader(_logger, _dataAccess);
            var cardData = InitTestCardInfos();

            loader.ImportCards(cardData);

            Assert.IsTrue(_cards.DataLoaded is not null
                && _cards.DataLoaded.Count() == cardData.Count);
        }

        [TestMethod]
        public void ImportCustomAttributes_ImportSingleAttributeSuccessfully()
        {
            InitTestVars();
            var productLineId = 62;
            var loader = new TcgCardLoader(_logger, _dataAccess);
            var cardData = InitTestCardInfos().Take(1);

            loader.ImportCustomAttributes(cardData, productLineId);

            Assert.IsTrue(_customAttributes.DataLoaded is not null
                && _customAttributes.DataLoaded.FirstOrDefault()?.ProductLineId == productLineId
                && _customAttributes.DataLoaded.FirstOrDefault()?.Name == cardData.First().CustomAttributes.First().Key);
        }

        [TestMethod]
        public void ImportCustomAttributes_ImportMultipleAttributesSuccessfullyIgnoringDuplicates()
        {
            InitTestVars();
            var productLineId = 62;
            var loader = new TcgCardLoader(_logger, _dataAccess);
            var cardData = InitTestCardInfos();
            var distinctAtts = cardData.SelectMany(data => data.CustomAttributes).DistinctBy(kvp => kvp.Key);

            loader.ImportCustomAttributes(cardData, productLineId);

            Assert.IsTrue(_customAttributes.DataLoaded is not null
                && _customAttributes.DataLoaded.All(ca => ca.ProductLineId == productLineId)
                && _customAttributes.DataLoaded.Count() == distinctAtts.Count());
        }

        [TestMethod]
        public void ImportCustomAttributesValues_ImportSingleCardAttributesValuesSuccessfully()
        {
            InitTestVars();
            var productLineId = 62;
            var loader = new TcgCardLoader(_logger, _dataAccess);
            var cardData = InitTestCardInfos().Take(1);
            var distinctAtts = cardData.SelectMany(data => data.CustomAttributes).DistinctBy(kvp => kvp.Key);
            var custAttsImport = distinctAtts.Select((atts, i) => new CustomAttribute()
            {
                CustomAttributeId = i,
                Name = atts.Key
            });

            _customAttributes.ImportCustomAttributes(custAttsImport);

            var attsDic = custAttsImport.ToDictionary(att => att.CustomAttributeId, att => att.Name);

            loader.ImportCustomAttributesValues(cardData, productLineId);

            Assert.IsTrue(_customAttributesValues.DataLoaded is not null
                && _customAttributesValues.DataLoaded.All(val => val.ProductId == cardData.First().ProductId.AsInt()
                    && cardData.First().CustomAttributes.Any(ca => ca.Value.AsString() == val.Value && ca.Key == attsDic[val.CustomAttributeId])));
        }

        [TestMethod]
        public void ImportCustomAttributesValues_ImportMultipleCardAttributesValuesSuccessfully()
        {
            InitTestVars();
            var productLineId = 62;
            var loader = new TcgCardLoader(_logger, _dataAccess);
            var cardData = InitTestCardInfos().DistinctBy(c => c.ProductId);
            var distinctAtts = cardData.SelectMany(data => data.CustomAttributes).DistinctBy(kvp => kvp.Key);
            var custAttsImport = distinctAtts.Select((atts, i) => new CustomAttribute()
            {
                CustomAttributeId = i,
                Name = atts.Key
            });

            _customAttributes.ImportCustomAttributes(custAttsImport);

            var attsDic = custAttsImport.ToDictionary(att => att.CustomAttributeId, att => att.Name);

            loader.ImportCustomAttributesValues(cardData, productLineId);

            Assert.IsTrue(_customAttributesValues.DataLoaded is not null
                && _customAttributesValues.DataLoaded.All(val => cardData.Any(c => c.ProductId.AsInt() == val.ProductId
                    && c.CustomAttributes.Any(ca => ca.Value.AsString() == val.Value && ca.Key == attsDic[val.CustomAttributeId]))));
        }

        public static List<CardInfo> InitTestCardInfos()
        {
            return new List<CardInfo> { new CardInfo()
            {
                ProductId = 1.0f,
                CustomAttributes = new Dictionary<string, JsonElement> { { "Test", default } }
            },
            new CardInfo() {
                ProductId = 2.0f,
                CustomAttributes = new Dictionary<string, JsonElement> { { "Test", default } }
            },
            new CardInfo() {
                ProductId = 3.0f,
                CustomAttributes = new Dictionary<string, JsonElement> { { "Test2", default } }
            },
            new CardInfo() {
                ProductId = 1.0f,
                CustomAttributes = new Dictionary<string, JsonElement> { { "Test3", default } }
            }};
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
