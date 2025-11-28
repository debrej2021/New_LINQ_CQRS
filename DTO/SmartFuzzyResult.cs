namespace DotNet_Quick_ref_all.DTO
{
    public class SmartFuzzyResult
    {
        public string Query { get; set; } = "";
        public string? Suggestion { get; set; }
        public List<object> Results { get; set; } = new();
    }
}
