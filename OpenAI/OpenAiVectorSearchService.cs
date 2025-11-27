using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace DotNet_Quick_ref_all.OpenAI
{
    public class OpenAiVectorSearchService
    {
        private readonly OpenAiEmbeddingService _embedder;

        // In-memory vector store
        private readonly ConcurrentDictionary<int, OpenAiVectorDocument> _vectorDb =
            new ConcurrentDictionary<int, OpenAiVectorDocument>();

        public OpenAiVectorSearchService(OpenAiEmbeddingService embedder)
        {
            _embedder = embedder;
        }

        // Add document to vector DB
        public async Task IndexAsync(int id, string text)
        {
            var emb = await _embedder.CreateEmbeddingAsync(text);

            _vectorDb[id] = new OpenAiVectorDocument
            {
                Id = id,
                Text = text,
                Embedding = emb
            };
        }

        // Bulk indexing
        public async Task IndexBulkAsync(IEnumerable<(int id, string text)> documents)
        {
            foreach (var doc in documents)
                await IndexAsync(doc.id, doc.text);
        }

        // Semantic vector search using cosine similarity
        public async Task<IEnumerable<object>> SearchAsync(string query, int topK = 5)
        {
            var qVec = await _embedder.CreateEmbeddingAsync(query);

            return _vectorDb.Values
      .Where(doc => doc.Embedding != null)                 // prevent null issues
      .Select(doc => new
      {
          doc.Id,
          doc.Text,
          Score = CosineSimilarity(qVec, doc.Embedding!)
      })
              .OrderByDescending(x => x.Score)
                .Take(topK)
                .ToList();
        }

        // Cosine similarity formula
        private float CosineSimilarity(float[] a, float[] b)
        {
            float dot = 0, magA = 0, magB = 0;

            int len = Math.Min(a.Length, b.Length); // safety for version differences

            for (int i = 0; i < len; i++)
            {
                dot += a[i] * b[i];
                magA += a[i] * a[i];
                magB += b[i] * b[i];
            }

            return dot / (float)(Math.Sqrt(magA) * Math.Sqrt(magB) + 1e-8);
        }
    }
}
