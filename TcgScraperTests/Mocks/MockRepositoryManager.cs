using DataAccess;
using DataAccess.Repositories;
using DataAccess.SqlModels;

namespace TcgScraperTests.Mocks
{
    internal class MockRepositoryManager : IRepositoryManager
    {
        public ICards Cards { get; set; } = new MockCards();
        public ICustomAttributes CustomAttributes { get; set; } = new MockCustomAttributesImport();
        public ICustomAttributesValues CustomAttributesValues { get; set; } = new MockCustomAttributesValues();

        public Dictionary<string, object> TestData = new();
    }

    internal class MockCards : ICards
    {
        public virtual List<Card> GetAllCards(int offset = 0, int limit = 100) { return new List<Card>(); }
        public virtual void ImportCards(IEnumerable<Card> cards) { }
    }

    internal class MockCustomAttributes : ICustomAttributes
    {
        public virtual List<CustomAttribute> GetAllAttributes() { return new List<CustomAttribute>(); }
        public virtual List<CustomAttribute> GetAttributesByProductLine(int productLineId) { return new List<CustomAttribute>(); }
        public virtual void ImportCustomAttributes(IEnumerable<CustomAttribute> atts) { }
    }

    internal class MockCustomAttributesValues : ICustomAttributesValues
    {
        public virtual List<CustomAttributesValue> GetCustomAttributesValues(int productId) { return new List<CustomAttributesValue>(); }
        public virtual void ImportCustomAttributesValues(IEnumerable<CustomAttributesValue> atts) { }
    }
}
