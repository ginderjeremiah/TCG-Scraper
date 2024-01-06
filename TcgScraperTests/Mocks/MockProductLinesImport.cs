using DataAccess.SqlModels;

namespace Tests.Mocks
{
    internal class MockProductLinesImport : MockProductLines
    {
        public Task? AwaitableTask { get; set; }
        public IEnumerable<ProductLine>? DataLoaded { get; set; }

        public override List<ProductLine> GetProductLines(int offset = 0, int limit = 100)
        {
            return DataLoaded?.Skip(offset).Take(limit).ToList() ?? new List<ProductLine>();
        }

        public override void ImportProductLines(IEnumerable<ProductLine> cards)
        {
            DataLoaded = cards;
            AwaitableTask?.Start();
        }
    }
}
