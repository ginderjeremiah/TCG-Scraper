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
        private IApiLogger Logger { get; set; }
        private IRepositoryManager Repositories { get; set; }

        public TcgCardLoader(IApiLogger logger, IRepositoryManager repositories)
        {
            Logger = logger;
            Repositories = repositories;
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
            Repositories.Cards.ImportCards(cards);
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

            Repositories.CustomAttributes.ImportCustomAttributes(uniqueCustomAtts);
        }

        public void ImportCustomAttributesValues(IEnumerable<CardInfo> cardData, int productLineId)
        {
            var customAttsDic = Repositories.CustomAttributes.GetAttributesByProductLine(productLineId)
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

            Repositories.CustomAttributesValues.ImportCustomAttributesValues(customAttValues);
        }

        public void ImportProductLines(IEnumerable<ApiModels.ProductLine> productLines)
        {
            var pLines = productLines.Select(NewProductLine);
            Repositories.ProductLines.ImportProductLines(pLines);
        }

        private Card NewCard(CardInfo cardInfo)
        {
            return new Card()
            {
                ProductId = cardInfo.ProductId.AsInt(),
                ShippingCategoryId = cardInfo.ShippingCategoryId.AsInt(),
                Duplicate = cardInfo.Duplicate,
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
                MaxFulfullableQuantity = cardInfo.MaxFulfullableQuantity.AsInt(),
                LowestPrice = cardInfo.LowestPrice
            };
        }

        private DataAccess.SqlModels.ProductLine NewProductLine(ApiModels.ProductLine productLine)
        {
            return new DataAccess.SqlModels.ProductLine()
            {
                ProductLineId = productLine.ProductLineId,
                ProductLineName = productLine.ProductLineName,
                ProductLineUrlName = productLine.ProductLineUrlName
            };
        }

        [GeneratedRegex("(?<l>[a-z])(?<u>[A-Z])")]
        private static partial Regex SpacedWordRegex();
    }
}
