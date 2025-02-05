﻿using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.SqlModels
{
    [Table("cards")]
    public class Card
    {
        [Column("product_id")]
        public int ProductId { get; set; }
        [Column("shipping_category_id")]
        public int ShippingCategoryId { get; set; }
        [Column("duplicate")]
        public bool Duplicate { get; set; }
        [Column("product_url_name")]
        public string ProductUrlName { get; set; }
        [Column("product_type_id")]
        public int ProductTypeId { get; set; }
        [Column("rarity_name")]
        public string? RarityName { get; set; }
        [Column("sealed")]
        public bool Sealed { get; set; }
        [Column("market_price")]
        public float MarketPrice { get; set; }
        [Column("lowest_price_with_shipping")]
        public float LowestPriceWithShipping { get; set; }
        [Column("product_name")]
        public string ProductName { get; set; }
        [Column("set_id")]
        public int SetId { get; set; }
        [Column("score")]
        public float? Score { get; set; }
        [Column("set_name")]
        public string SetName { get; set; }
        [Column("foil_only")]
        public bool FoilOnly { get; set; }
        [Column("set_url_name")]
        public string SetUrlName { get; set; }
        [Column("seller_listable")]
        public bool SellerListable { get; set; }
        [Column("total_listings")]
        public int TotalListings { get; set; }
        [Column("product_line_id")]
        public int ProductLineId { get; set; }
        [Column("product_status_id")]
        public int ProductStatusId { get; set; }
        [Column("max_fulfullable_quantity")]
        public int MaxFulfullableQuantity { get; set; }
        [Column("lowest_price")]
        public float LowestPrice { get; set; }
    }
}
