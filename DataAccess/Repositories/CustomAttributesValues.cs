using DataAccess.SqlModels;
using Npgsql;
using System.Data;

namespace DataAccess.Repositories
{
    public static class CustomAttributesValues
    {
        public static List<CustomAttributesValue> GetCustomAttributesValues(int productId)
        {
            DataTable dt = new();
            dt.Fill("SELECT * FROM custom_attributes_values WHERE product_id = @product_id", new NpgsqlParameter("product_id", productId));
            return dt.To<CustomAttributesValue>();
        }

        public static void ImportCustomAttributesValues(IEnumerable<CustomAttributesValue> atts)
        {
            DataTable dt = new();
            dt.Fill(atts);
            dt.CopyToSqlTable("custom_attributes_values_staging", "iu_sp_import_custom_attributes_values_staging");
        }
    }
}
