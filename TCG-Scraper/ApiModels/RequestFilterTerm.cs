namespace ApiModels
{
    public class RequestFilterTerm
    {
        public List<string> ProductLineName { get; set; }
        public List<string> SetName { get; set; }
        public RequestFilterTerm(string productLineName, string setName)
        {
            ProductLineName = new List<string> { productLineName };
            SetName = new List<string> { setName };
        }
    }
}
