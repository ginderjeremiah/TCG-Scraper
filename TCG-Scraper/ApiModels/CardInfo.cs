namespace ApiModels
{
    class CardInfo
    {
        public float ShippingCategoryId { get; set; }
        public bool Duplicate { get; set; }
        public string ProductLineUrlName { get; set; }
        public string ProductUrlName { get; set; }
        public float ProductTypeId { get; set; }
        public string RarityName { get; set; }
        public bool Sealed { get; set; }
        public float MarketPrice { get; set; }
        public CustomAttributes CustomAttributes { get; set; }
        public float LowestPriceWithShipping { get; set; }
        public string ProductName { get; set; }
        public float SetId { get; set; } //not sure why this isn't an int
        public float ProductId { get; set; } //not sure why this isn't an int
        public float Score { get; set; }
        public string SetName { get; set; }
        public bool FoilOnly { get; set; }
        public string SetUrlName { get; set; }
        public bool SellerListable { get; set; }
        public float TotalListings { get; set; } //not sure why this isn't an int
        public float ProductLineId { get; set; } //not sure why this isn't an int
        public float ProductStatusId { get; set; } //not sure why this isn't an int
        public string ProductLineName { get; set; }
        public float MaxFulfullableQuantity { get; set; }
        public List<CardListing> Listings { get; set; }
        public float LowestPrice { get; set; }
    }
}
