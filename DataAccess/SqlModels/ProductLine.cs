using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.SqlModels
{
    [Table("product_lines")]
    public class ProductLine
    {
        [Column("product_line_id")]
        public int ProductLineId { get; set; }
        [Column("product_line_name")]
        public string ProductLineName { get; set; }
        [Column("product_line_url_name")]
        public string ProductLineUrlName { get; set; }
    }
}
