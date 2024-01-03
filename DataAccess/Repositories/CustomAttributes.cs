using DataAccess.SqlModels;
using Npgsql;
using System.Data;

namespace DataAccess.Repositories
{
    public class CustomAttributes : ICustomAttributes
    {
        public List<CustomAttribute> GetAllAttributes()
        {
            DataTable dt = new();
            dt.Fill("SELECT * FROM custom_attributes");
            return dt.To<CustomAttribute>();
        }

        public List<CustomAttribute> GetAttributesByProductLine(int productLineId)
        {
            DataTable dt = new();
            dt.Fill("SELECT * FROM custom_attributes WHERE product_line_id = @product_line_id", new NpgsqlParameter("product_line_id", productLineId));
            return dt.To<CustomAttribute>();
        }

        public void ImportCustomAttributes(IEnumerable<CustomAttribute> atts)
        {
            DataTable dt = new();
            dt.Fill(atts, false);
            dt.CopyToSqlTable("custom_attributes_staging", "i_sp_import_custom_attributes_staging");
        }
    }

    public interface ICustomAttributes
    {
        public List<CustomAttribute> GetAllAttributes();
        public List<CustomAttribute> GetAttributesByProductLine(int productLineId);
        public void ImportCustomAttributes(IEnumerable<CustomAttribute> atts);
    }
}
