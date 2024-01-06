using DataAccess.SqlModels;
using TCG_Scraper;
using Tests.Mocks;

namespace Tests
{
    [TestClass]
    public class TcgCardLoaderTests
    {
        private MockLogger _logger = new();
        private MockRepositoryManager _dataAccess = new();
        private MockCardsImport _cards = new();
        private MockCustomAttributesImport _customAttributes = new();
        private MockCustomAttributesValuesImport _customAttributesValues = new();

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
        public void ImportAllCardData_SingleCard_LoadsSuccessfully()
        {
            InitTestVars();
            var productLineId = 62;
            var loader = new TcgCardLoader(_logger, _dataAccess);
            var cardData = MockCardsImport.InitTestCardInfos().Take(1);

            loader.ImportAllCardData(cardData, productLineId);
            Assert.IsTrue(_cards.DataLoaded is not null
                && _cards.DataLoaded.FirstOrDefault()?.ProductId == 1);

            Assert.IsTrue(_customAttributes.DataLoaded is not null
                && _customAttributes.DataLoaded.FirstOrDefault()?.ProductLineId == productLineId
                && _customAttributes.DataLoaded.FirstOrDefault()?.Name == cardData.First().CustomAttributes.First().Key);

            Assert.IsTrue(_customAttributesValues.DataLoaded is not null
                && _customAttributesValues.DataLoaded.FirstOrDefault()?.ProductId == cardData.First().ProductId.AsInt());
        }

        public void ImportAllCardData_MultipleCardsWithDuplicates_LoadsSuccessfullyIgnoringDuplicates()
        {
            InitTestVars();
            var productLineId = 62;
            var loader = new TcgCardLoader(_logger, _dataAccess);
            var cardData = MockCardsImport.InitTestCardInfos();
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
        public void ImportCards_SingleCard_LoadsSuccessfully()
        {
            InitTestVars();
            var loader = new TcgCardLoader(_logger, _dataAccess);
            var cardData = MockCardsImport.InitTestCardInfos().Take(1);

            loader.ImportCards(cardData);

            Assert.IsTrue(_cards.DataLoaded is not null
                && _cards.DataLoaded.FirstOrDefault()?.ProductId == cardData.First().ProductId.AsInt());
        }

        [TestMethod]
        public void ImportCards_MultipleCards_LoadsSuccessfully()
        {
            InitTestVars();
            var loader = new TcgCardLoader(_logger, _dataAccess);
            var cardData = MockCardsImport.InitTestCardInfos();

            loader.ImportCards(cardData);

            Assert.IsTrue(_cards.DataLoaded is not null
                && _cards.DataLoaded.Count() == cardData.Count);
        }

        [TestMethod]
        public void ImportCustomAttributes_SingleAttribute_ImportsSuccessfully()
        {
            InitTestVars();
            var productLineId = 62;
            var loader = new TcgCardLoader(_logger, _dataAccess);
            var cardData = MockCardsImport.InitTestCardInfos().Take(1);

            loader.ImportCustomAttributes(cardData, productLineId);

            Assert.IsTrue(_customAttributes.DataLoaded is not null
                && _customAttributes.DataLoaded.FirstOrDefault()?.ProductLineId == productLineId
                && _customAttributes.DataLoaded.FirstOrDefault()?.Name == cardData.First().CustomAttributes.First().Key);
        }

        [TestMethod]
        public void ImportCustomAttributes_MultipleAttributesWithDuplicates_ImportsSuccessfullyIgnoringDuplicates()
        {
            InitTestVars();
            var productLineId = 62;
            var loader = new TcgCardLoader(_logger, _dataAccess);
            var cardData = MockCardsImport.InitTestCardInfos();
            var distinctAtts = cardData.SelectMany(data => data.CustomAttributes).DistinctBy(kvp => kvp.Key);

            loader.ImportCustomAttributes(cardData, productLineId);

            Assert.IsTrue(_customAttributes.DataLoaded is not null
                && _customAttributes.DataLoaded.All(ca => ca.ProductLineId == productLineId)
                && _customAttributes.DataLoaded.Count() == distinctAtts.Count());
        }

        [TestMethod]
        public void ImportCustomAttributesValues_SingleCardAttributesValues_ImportSuccessfully()
        {
            InitTestVars();
            var productLineId = 62;
            var loader = new TcgCardLoader(_logger, _dataAccess);
            var cardData = MockCardsImport.InitTestCardInfos().Take(1);
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
        public void ImportCustomAttributesValues_MultipleCardAttributesValues_ImportsSuccessfully()
        {
            InitTestVars();
            var productLineId = 62;
            var loader = new TcgCardLoader(_logger, _dataAccess);
            var cardData = MockCardsImport.InitTestCardInfos().DistinctBy(c => c.ProductId);
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
    }
}
