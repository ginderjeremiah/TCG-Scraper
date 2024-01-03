using DataAccess.SqlModels;
using System.Data;

namespace DataAccess.Repositories
{
    public class Cards : ICards
    {
        public List<Card> GetAllCards()
        {
            DataTable dt = new();
            dt.Fill("SELECT * FROM cards");
            return dt.To<Card>();
        }

        public void ImportCards(IEnumerable<Card> cards)
        {
            DataTable dt = new();
            dt.Fill(cards);
            dt.CopyToSqlTable("cards_staging", "iu_sp_import_cards_staging");
        }
    }

    public interface ICards
    {
        public List<Card> GetAllCards();
        public void ImportCards(IEnumerable<Card> cards);
    }
}
