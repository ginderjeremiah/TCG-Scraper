using DataAccess;
using DataAccess.Repositories;
using DataAccess.SqlModels;

namespace TcgScraperTests.Mocks
{
    internal class TestDataAccess : IDataAccess
    {
        public ICards Cards { get; set; } = new TestCards();
        public ICustomAttributes CustomAttributes { get; set; } = new TestCustomAttributesImport();
        public ICustomAttributesValues CustomAttributesValues { get; set; } = new TestCustomAttributesValues();

        public Dictionary<string, object> TestData = new();
    }

    internal class TestCards : ICards
    {
        public virtual List<Card> GetAllCards() { return new List<Card>(); }
        public virtual void ImportCards(IEnumerable<Card> cards) { }
    }

    internal class TestCustomAttributes : ICustomAttributes
    {
        public virtual List<CustomAttribute> GetAllAttributes() { return new List<CustomAttribute>(); }
        public virtual List<CustomAttribute> GetAttributesByProductLine(int productLineId) { return new List<CustomAttribute>(); }
        public virtual void ImportCustomAttributes(IEnumerable<CustomAttribute> atts) { }
    }

    internal class TestCustomAttributesValues : ICustomAttributesValues
    {
        public virtual List<CustomAttributesValue> GetCustomAttributesValues(int productId) { return new List<CustomAttributesValue>(); }
        public virtual void ImportCustomAttributesValues(IEnumerable<CustomAttributesValue> atts) { }
    }
}
