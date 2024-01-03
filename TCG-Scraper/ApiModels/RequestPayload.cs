namespace ApiModels
{

    public class RequestPayload
    {
        //public string Algorithm { get; set; } = "sales_exp_fields_experiment";
        //public RequestContext Context { get; set; }
        public RequestFilters Filters { get; set; }
        public int From { get; set; } = 0;
        //public RequestListingSearch ListingSearch { get; set; }
        //public RequestSettings Settings { get; set; }
        public int Size { get; set; } = 24;
        //public object Sort { get; set; }

        public RequestPayload(string productLineName, string setName, int start, int size)
        {
            Filters = new RequestFilters(productLineName, setName);
            Size = size;
            From = start;
        }
    }
}
