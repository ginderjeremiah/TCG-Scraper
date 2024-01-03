namespace ApiModels
{
    public class SetInfo
    {
        public int SetNameId { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string CleanSetName { get; set; }
        public string UrlName { get; set; }
        public string Abbreviation { get; set; }
        public DateTime ReleaseDate { get; set; }
        public bool Active { get; set; }
    }
}
