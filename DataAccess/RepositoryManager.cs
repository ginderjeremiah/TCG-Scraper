using DataAccess.Repositories;

namespace DataAccess
{
    public class RepositoryManager : IRepositoryManager
    {
        private Cards? _cards;
        private CustomAttributes? _customAttributes;
        private CustomAttributesValues? _customAttributesValues;
        private string ConnectionString { get; set; }

        public RepositoryManager(ConnectionSettings connectionSettings)
        {
            ConnectionString = connectionSettings.GetConnectionString();
        }

        public ICards Cards => _cards ??= new Cards(ConnectionString);
        public ICustomAttributes CustomAttributes => _customAttributes ??= new CustomAttributes(ConnectionString);
        public ICustomAttributesValues CustomAttributesValues => _customAttributesValues ??= new CustomAttributesValues(ConnectionString);
    }

    public interface IRepositoryManager
    {
        public ICards Cards { get; }
        public ICustomAttributes CustomAttributes { get; }
        public ICustomAttributesValues CustomAttributesValues { get; }
    }
}


