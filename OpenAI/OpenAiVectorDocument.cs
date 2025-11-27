namespace DotNet_Quick_ref_all.OpenAI
{
    public class OpenAiVectorDocument
    {
        public int Id { get; set; }
        public string Text { get; set; } = "";
        public float[]? Embedding { get; set; }
    }
}
