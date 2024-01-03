using DataAccess.Repositories;

namespace DataAccess
{
    public class DataAccess : IDataAccess
    {
        Cards? _cards;
        CustomAttributes? _customAttributes;
        CustomAttributesValues? _customAttributesValues;

        public ICards Cards => _cards ??= new Cards();
        public ICustomAttributes CustomAttributes => _customAttributes ??= new CustomAttributes();
        public ICustomAttributesValues CustomAttributesValues => _customAttributesValues ??= new CustomAttributesValues();
    }

    public interface IDataAccess
    {
        public ICards Cards { get; }
        public ICustomAttributes CustomAttributes { get; }
        public ICustomAttributesValues CustomAttributesValues { get; }
    }
}


