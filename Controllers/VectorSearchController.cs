using DotNet_Quick_ref_all.OpenAI;
using Microsoft.AspNetCore.Mvc;

namespace DotNet_Quick_ref_all.Controllers
{
    [ApiController]
    [Route("api/vector")]
    public class VectorSearchController : ControllerBase
    {
        private readonly OpenAiVectorSearchService _vector;

        public VectorSearchController(OpenAiVectorSearchService vector)
        {
            _vector = vector;
        }

        // 1️⃣ Bulk index (for DB → vector store)
        [HttpPost("index-all")]
        public async Task<IActionResult> IndexAll([FromBody] List<IndexRequest> docs)
        {
            await _vector.IndexBulkAsync(
                docs.Select(d => (d.Id, d.Text))
            );

            return Ok("Vector index completed.");
        }

        // 2️⃣ Semantic search
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string query)
        {
            var result = await _vector.SearchAsync(query);
            return Ok(result);
        }
    }
    /// <summary>
    /// index request model later in nosql db
    /// </summary>
    public class IndexRequest
    {
        public int Id { get; set; }
        public string? Text { get; set; }
    }
}
