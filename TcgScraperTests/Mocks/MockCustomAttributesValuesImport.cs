using DataAccess.SqlModels;

namespace Tests.Mocks
{
    internal class MockCustomAttributesValuesImport : MockCustomAttributesValues
    {
        public Task? AwaitableTask { get; set; }
        public IEnumerable<CustomAttributesValue>? DataLoaded { get; set; }
        public override void ImportCustomAttributesValues(IEnumerable<CustomAttributesValue> atts)
        {
            DataLoaded = atts;
            AwaitableTask?.Start();
        }
    }
}
