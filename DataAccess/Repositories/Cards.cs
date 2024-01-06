using DataAccess.SqlModels;
using Npgsql;

namespace DataAccess.Repositories
{
    public class Cards : BaseRepository, ICards
    {
        public Cards(string connectionString) : base(connectionString) { }

        public List<Card> GetCards(int offset = 0, int limit = 100)
        {
            limit = Math.Min(limit, 1000);

            return QueryToList<Card>("SELECT * FROM cards LIMIT @Limit OFFSET @Offset",
                new NpgsqlParameter("@Limit", limit),
                new NpgsqlParameter("@Offset", offset));
        }

        public void ImportCards(IEnumerable<Card> cards)
        {
            CopyToSqlTable(cards, "cards_staging", "iu_sp_import_cards_staging");
        }
    }

    public interface ICards
    {
        public List<Card> GetCards(int offset = 0, int limit = 100);
        public void ImportCards(IEnumerable<Card> cards);
    }
}
