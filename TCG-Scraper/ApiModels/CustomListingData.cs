namespace ApiModels
{
    public class CustomListingData
    {
        public List<string> Images { get; set; } //not sure what appropriate data type is... assuming these are img URLs

        public override string ToString()
        {
            return string.Join(", ", Images);
        }
    }
}
