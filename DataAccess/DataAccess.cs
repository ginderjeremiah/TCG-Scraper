using DataAccess.Repositories;

namespace DataAccess
{
    public class DataAccess : IDataAccess
    {
        private Cards? _cards;
        private CustomAttributes? _customAttributes;
        private CustomAttributesValues? _customAttributesValues;
        private string ConnectionString { get; set; }

        public DataAccess(ConnectionSettings connectionSettings)
        {
            ConnectionString = connectionSettings.GetConnectionString();
        }

        public ICards Cards => _cards ??= new Cards(ConnectionString);
        public ICustomAttributes CustomAttributes => _customAttributes ??= new CustomAttributes(ConnectionString);
        public ICustomAttributesValues CustomAttributesValues => _customAttributesValues ??= new CustomAttributesValues(ConnectionString);
    }

    public interface IDataAccess
    {
        public ICards Cards { get; }
        public ICustomAttributes CustomAttributes { get; }
        public ICustomAttributesValues CustomAttributesValues { get; }
    }
}


