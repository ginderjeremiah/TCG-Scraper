using DataAccess.SqlModels;
using System.Data;

namespace DataAccess.Repositories
{
    public static class Cards
    {
        public static List<Card> GetAllCards()
        {
            DataTable dt = new();
            dt.Fill("SELECT * FROM cards");
            return dt.To<Card>();
        }

        public static void ImportCards(IEnumerable<Card> cards)
        {
            DataTable dt = new();
            dt.Fill(cards);
            dt.CopyToSqlTable("cards_staging", "iu_sp_import_cards_staging");
        }
    }
}
