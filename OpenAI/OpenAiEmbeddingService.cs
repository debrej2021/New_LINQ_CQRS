using OpenAI.Embeddings;

namespace DotNet_Quick_ref_all.OpenAI
{
    public class OpenAiEmbeddingService
    {
        private readonly EmbeddingClient _embeddings;

        public OpenAiEmbeddingService(IConfiguration config)
        {
            string apiKey = config["OpenAI:ApiKey"];

            // MUST include model name + apiKey (OpenAI v2.x requirement)
            _embeddings = new EmbeddingClient("text-embedding-3-small", apiKey);
        }

        public async Task<float[]> CreateEmbeddingAsync(string text)
        {
            text ??= "";

            // Correct method in v2.x
            var result = await _embeddings.GenerateEmbeddingAsync(text);



            // Use ToFloats() to get the embedding vector
            return result.Value.ToFloats().ToArray();
        }
    }
}
