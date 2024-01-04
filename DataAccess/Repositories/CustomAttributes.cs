using DataAccess.SqlModels;
using Npgsql;

namespace DataAccess.Repositories
{
    public class CustomAttributes : BaseRepository, ICustomAttributes
    {
        public CustomAttributes(string connectionString) : base(connectionString) { }

        public List<CustomAttribute> GetAllAttributes()
        {
            return QueryToList<CustomAttribute>("SELECT * FROM custom_attributes");
        }

        public List<CustomAttribute> GetAttributesByProductLine(int productLineId)
        {
            return QueryToList<CustomAttribute>(
                "SELECT * FROM custom_attributes WHERE product_line_id = @product_line_id",
                new NpgsqlParameter("product_line_id", productLineId)
            );
        }

        public void ImportCustomAttributes(IEnumerable<CustomAttribute> atts)
        {
            CopyToSqlTable(atts, "custom_attributes_staging", "i_sp_import_custom_attributes_staging");
        }
    }

    public interface ICustomAttributes
    {
        public List<CustomAttribute> GetAllAttributes();
        public List<CustomAttribute> GetAttributesByProductLine(int productLineId);
        public void ImportCustomAttributes(IEnumerable<CustomAttribute> atts);
    }
}
