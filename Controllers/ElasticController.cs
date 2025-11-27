using DotNet_Quick_ref_all.Elastic;
using Microsoft.AspNetCore.Mvc;

namespace DotNet_Quick_ref_all.Controllers
{
    [ApiController]
    [Route("es")]
    public class ElasticController : ControllerBase
    {
        private readonly ElasticIndexerService _indexer;
        private readonly ElasticSearchService _search;

        public ElasticController(
            ElasticIndexerService indexer,
            ElasticSearchService search)
        {
            _indexer = indexer;
            _search = search;
        }

        /// <summary>
        /// reindex all data from DB to Elasticsearch
        /// </summary>
        /// <returns></returns>
        // 1️⃣ Reindex all DB → Elastic
        [HttpPost("reindex")]
        public async Task<IActionResult> Reindex()
        {
            await _indexer.ReindexAsync();
            return Ok("Reindex completed.");
        }

        // 2️⃣ Full-text search
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string query)
        {
            var result = await _search.SearchAsync(query);
            return Ok(result);
        }

        // 3️⃣ Category-only filter
        [HttpGet("category")]
        public async Task<IActionResult> SearchByCategory([FromQuery] string name)
        {
            var result = await _search.SearchByCategoryAsync(name);
            return Ok(result);
        }
    }
}
