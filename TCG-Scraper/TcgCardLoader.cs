using ApiModels;
using CommonLibrary;
using DataAccess;
using DataAccess.SqlModels;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace TCG_Scraper
{
    public partial class TcgCardLoader
    {
        private ILogger Logger { get; set; }
        private IDataAccess DataAccess { get; set; }

        public TcgCardLoader(ILogger logger, IDataAccess dataAccess)
        {
            Logger = logger;
            DataAccess = dataAccess;
        }

        public void ImportAllCardData(IEnumerable<CardInfo> cardData, int productLineId)
        {
            var uniqueCardData = cardData.DistinctBy(cardInfo => cardInfo.ProductId);
            ImportCards(uniqueCardData);
            ImportCustomAttributes(uniqueCardData, productLineId);
            ImportCustomAttributesValues(uniqueCardData, productLineId);
        }

        public void ImportCards(IEnumerable<CardInfo> cardData)
        {
            var cards = cardData.Select(NewCard);
            DataAccess.Cards.ImportCards(cards);
        }

        public void ImportCustomAttributes(IEnumerable<CardInfo> cardData, int productLineId)
        {
            var uniqueCustomAtts = cardData.SelectMany(data => data.CustomAttributes.Where(att => att.Value.ValueKind != JsonValueKind.Null))
                .DistinctBy(att => att.Key)
                .Select(att => new CustomAttribute()
                {
                    Name = att.Key,
                    DisplayName = SpacedWordRegex().Replace(att.Key, "${l} ${u}"),
                    DataType = att.Value.ValueKind.ToString(),
                    ProductLineId = productLineId
                });

            DataAccess.CustomAttributes.ImportCustomAttributes(uniqueCustomAtts);
        }

        public void ImportCustomAttributesValues(IEnumerable<CardInfo> cardData, int productLineId)
        {
            var customAttsDic = DataAccess.CustomAttributes.GetAttributesByProductLine(productLineId)
                .ToDictionary(att => att.Name, att => att.CustomAttributeId);

            var customAttValues = cardData
                .SelectMany(data => data.CustomAttributes
                .Where(atts => atts.Value.ValueKind != JsonValueKind.Null)
                .Select(atts => new CustomAttributesValue()
                {
                    ProductId = data.ProductId.AsInt(),
                    CustomAttributeId = customAttsDic[atts.Key],
                    Value = atts.Value.AsString()
                }));

            DataAccess.CustomAttributesValues.ImportCustomAttributesValues(customAttValues);
        }

        private Card NewCard(CardInfo cardInfo)
        {
            return new Card()
            {
                ProductId = cardInfo.ProductId.AsInt(),
                ShippingCategoryId = cardInfo.ShippingCategoryId.AsInt(),
                Duplicate = cardInfo.Duplicate,
                ProductLineUrlName = cardInfo.ProductLineUrlName,
                ProductUrlName = cardInfo.ProductUrlName,
                ProductTypeId = cardInfo.ProductTypeId.AsInt(),
                RarityName = cardInfo.RarityName,
                Sealed = cardInfo.Sealed,
                MarketPrice = cardInfo.MarketPrice,
                LowestPriceWithShipping = cardInfo.LowestPriceWithShipping,
                ProductName = cardInfo.ProductName,
                SetId = cardInfo.SetId.AsInt(),
                Score = cardInfo.Score,
                SetName = cardInfo.SetName,
                FoilOnly = cardInfo.FoilOnly,
                SetUrlName = cardInfo.SetUrlName,
                SellerListable = cardInfo.SellerListable,
                TotalListings = cardInfo.TotalListings.AsInt(),
                ProductLineId = cardInfo.ProductLineId.AsInt(),
                ProductStatusId = cardInfo.ProductStatusId.AsInt(),
                ProductLineName = cardInfo.ProductLineName,
                MaxFulfullableQuantity = cardInfo.MaxFulfullableQuantity.AsInt(),
                LowestPrice = cardInfo.LowestPrice
            };
        }

        [GeneratedRegex("(?<l>[a-z])(?<u>[A-Z])")]
        private static partial Regex SpacedWordRegex();
    }
}
