using ApiModels;
using DataAccess.SqlModels;
using System.Text.Json;

namespace Tests.Mocks
{
    internal class MockCardsImport : MockCards
    {
        public Task? AwaitableTask { get; set; }
        public IEnumerable<Card>? DataLoaded { get; set; }

        public override List<Card> GetAllCards(int offset = 0, int limit = 100)
        {
            return DataLoaded?.Skip(offset).Take(limit).ToList() ?? new List<Card>();
        }

        public override void ImportCards(IEnumerable<Card> cards)
        {
            DataLoaded = cards;
            AwaitableTask?.Start();
        }

        public static List<CardInfo> InitTestCardInfos()
        {
            return new List<CardInfo> { new CardInfo()
            {
                ProductId = 1.0f,
                CustomAttributes = new Dictionary<string, JsonElement> { { "Test", default } }
            },
            new CardInfo() {
                ProductId = 2.0f,
                CustomAttributes = new Dictionary<string, JsonElement> { { "Test", default } }
            },
            new CardInfo() {
                ProductId = 3.0f,
                CustomAttributes = new Dictionary<string, JsonElement> { { "Test2", default } }
            },
            new CardInfo() {
                ProductId = 1.0f,
                CustomAttributes = new Dictionary<string, JsonElement> { { "Test3", default } }
            }};
        }

        public static List<Card> InitTestCards()
        {
            return new List<Card> { new Card()
            {
                ProductId = 1
            },
            new Card() {
                ProductId = 2
            },
            new Card() {
                ProductId = 3
            },
            new Card() {
                ProductId = 4
            }};
        }
    }
}
