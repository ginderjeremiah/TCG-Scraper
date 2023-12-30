namespace ApiModels
{
    class CardListing
    {
        public bool DirectProduct { get; set; }
        public bool GoldSeller { get; set; }
        public float ListingId { get; set; } //not sure why this isn't an int
        public float ChannelId { get; set; } //not sure why this isn't an int
        public float ConditionId { get; set; } //not sure why this isn't an int
        public bool VerifiedSeller { get; set; }
        public float DirectInventory { get; set; }
        public float RankedShippingPrice { get; set; }
        public float ProductId { get; set; } //not sure why this isn't an int
        public string Printing { get; set; }
        public string LanguageAbbreviation { get; set; }
        public string SellerName { get; set; }
        public bool ForwardFreight { get; set; }
        public float SellerShippingPrice { get; set; }
        public string Language { get; set; }
        public float ShippingPrice { get; set; }
        public string Condition { get; set; }
        public float LanguageId { get; set; } //not sure why this isn't an int
        public float Score { get; set; }
        public bool DirectSeller { get; set; }
        public float ProductConditionId { get; set; } //not sure why this isn't an int
        public string SellerId { get; set; } //not sure why this isn't an int
        public string ListingType { get; set; }
        public float SellerRating { get; set; }
        public string SellerSales { get; set; }
        public float Quantity { get; set; } //not sure why this isn't an int
        public string SellerKey { get; set; }
        public float Price { get; set; }
        public CustomListingData CustomData { get; set; }
    }
}
