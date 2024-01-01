using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.SqlModels
{
    [Table("custom_attributes")]
    public class CustomAttribute
    {
        [Column("custom_attribute_id")]
        public int CustomAttributeId { get; set; }
        [Column("name")]
        public string Name { get; set; }
    }
}
