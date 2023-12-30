namespace ApiModels
{
    class RequestFilters
    {
        //public object Match { get; set; } = new();
        //public object Range { get; set; } = new();
        public RequestFilterTerm Term { get; set; }
        public RequestFilters(string productLineName, string setName)
        {
            Term = new RequestFilterTerm(productLineName, setName);
        }
    }
}
