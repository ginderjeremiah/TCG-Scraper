using DataAccess.SqlModels;
using Npgsql;

namespace DataAccess.Repositories
{
    public class CustomAttributesValues : BaseRepository, ICustomAttributesValues
    {
        public CustomAttributesValues(string connectionString) : base(connectionString) { }

        public List<CustomAttributesValue> GetCustomAttributesValues(int productId)
        {
            return QueryToList<CustomAttributesValue>(
                "SELECT * FROM custom_attributes_values WHERE product_id = @product_id",
                new NpgsqlParameter("product_id", productId)
            );
        }

        public void ImportCustomAttributesValues(IEnumerable<CustomAttributesValue> atts)
        {
            CopyToSqlTable(atts, "custom_attributes_values_staging", "iu_sp_import_custom_attributes_values_staging");
        }
    }

    public interface ICustomAttributesValues
    {
        public List<CustomAttributesValue> GetCustomAttributesValues(int productId);
        public void ImportCustomAttributesValues(IEnumerable<CustomAttributesValue> atts);
    }
}
