using DataAccess.SqlModels;

namespace TcgScraperTests.Mocks
{
    internal class MockCardsImport : MockCards
    {
        public Task? AwaitableTask { get; set; }
        public IEnumerable<Card>? DataLoaded { get; set; }
        public override void ImportCards(IEnumerable<Card> cards)
        {
            DataLoaded = cards;
            AwaitableTask?.Start();
        }
    }
}
