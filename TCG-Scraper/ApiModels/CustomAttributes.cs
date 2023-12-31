namespace ApiModels
{
    class CustomAttributes
    {
        public string Description { get; set; }
        public string DetailNote { get; set; }
        public string? Intellect { get; set; } //not sure what data type this should be... possibly List<string>
        public DateTime? ReleaseDate { get; set; }
        public string Number { get; set; }
        public List<string> Talent { get; set; }
        public string PitchValue { get; set; }
        public List<string> CardType { get; set; }
        public string DefenseValue { get; set; }
        public string RarityDbName { get; set; }
        public string? Life { get; set; } //not sure what data type this should be... possibly List<string>
        public List<string> CardSubType { get; set; }
        public string Power { get; set; }
        public string? FlavorText { get; set; }
        public List<string> Class { get; set; }
        public string Cost { get; set; }
    }
}
