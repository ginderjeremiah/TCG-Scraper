using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.SqlModels
{
    [Table("custom_attributes_values")]
    public class CustomAttributesValue
    {
        [Column("custom_attribute_id")]
        public int CustomAttributeId { get; set; }
        [Column("product_id")]
        public int ProductId { get; set; }
        [Column("value")]
        public string Value { get; set; }
    }
}
