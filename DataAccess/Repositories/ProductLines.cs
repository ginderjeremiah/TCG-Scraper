using DataAccess.SqlModels;
using Npgsql;

namespace DataAccess.Repositories
{
    public class ProductLines : BaseRepository, IProductLines
    {
        public ProductLines(string connectionString) : base(connectionString) { }

        public List<ProductLine> GetProductLines(int offset = 0, int limit = 100)
        {
            limit = Math.Min(limit, 1000);

            return QueryToList<ProductLine>("SELECT * FROM product_lines LIMIT @Limit OFFSET @Offset",
                new NpgsqlParameter("@Limit", limit),
                new NpgsqlParameter("@Offset", offset));
        }

        public void ImportProductLines(IEnumerable<ProductLine> cards)
        {
            CopyToSqlTable(cards, "product_lines_staging", "iu_sp_import_product_lines_staging");
        }
    }

    public interface IProductLines
    {
        public List<ProductLine> GetProductLines(int offset = 0, int limit = 100);
        public void ImportProductLines(IEnumerable<ProductLine> cards);
    }
}
