namespace ApiModels
{
    public class CardInfoResults
    {
        //public RequestResultsAggregations Aggregations { get; set; }
        //public string Algorithm { get; set; }
        //public string SearchType { get; set; }
        //public object DidYouMean { get; set; }
        public int TotalResults { get; set; }
        //public string ResultId { get; set; }
        public List<CardInfo> Results { get; set; }
    }
}
