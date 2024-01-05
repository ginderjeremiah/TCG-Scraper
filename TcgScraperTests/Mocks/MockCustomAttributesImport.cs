using DataAccess.SqlModels;

namespace TcgScraperTests.Mocks
{
    internal class MockCustomAttributesImport : MockCustomAttributes
    {
        public Task? AwaitableTask { get; set; }
        public IEnumerable<CustomAttribute>? DataLoaded { get; set; }
        public override void ImportCustomAttributes(IEnumerable<CustomAttribute> atts)
        {
            DataLoaded = atts.Select((att, i) =>
            {
                att.CustomAttributeId = i;
                return att;
            });
            AwaitableTask?.Start();
        }

        public override List<CustomAttribute> GetAttributesByProductLine(int productLineId)
        {
            return DataLoaded?.ToList() ?? new List<CustomAttribute>();
        }
    }
}
