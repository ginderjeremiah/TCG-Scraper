namespace ApiModels
{
    class RequestContext
    {
        public RequestContextCart Cart { get; set; } = new();
        public string ShippingCountry { get; set; } = "US";
        public RequestUserProfile UserProfile { get; set; } = new();
    }
}
