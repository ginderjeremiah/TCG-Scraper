namespace ApiModels
{
    public class RequestResponse<T>
    {
        public List<string> Errors { get; set; }
        public List<T> Results { get; set; }
    }
}