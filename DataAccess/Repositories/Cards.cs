using DataAccess.SqlModels;

namespace DataAccess.Repositories
{
    public class Cards : BaseRepository, ICards
    {
        public Cards(string connectionString) : base(connectionString) { }

        public List<Card> GetAllCards()
        {
            return QueryToList<Card>("SELECT * FROM cards");
        }

        public void ImportCards(IEnumerable<Card> cards)
        {
            CopyToSqlTable(cards, "cards_staging", "iu_sp_import_cards_staging");
        }
    }

    public interface ICards
    {
        public List<Card> GetAllCards();
        public void ImportCards(IEnumerable<Card> cards);
    }
}
