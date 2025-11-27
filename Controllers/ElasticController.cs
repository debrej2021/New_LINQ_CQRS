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

        // --------------------------------------------------------------------
        // 1️⃣ Reindex DB → Elasticsearch
        // --------------------------------------------------------------------
        [HttpPost("reindex")]
        public async Task<IActionResult> Reindex()
        {
            await _indexer.ReindexAsync();
            return Ok(new { message = "Reindex completed successfully." });
        }

        // --------------------------------------------------------------------
        // 2️⃣ Full text + fuzzy + boosted multi-field search
        // --------------------------------------------------------------------
        [HttpGet("search")]
        public async Task<IActionResult> Search(
            [FromQuery] string query,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _search.SearchAsync(query, page, pageSize);
            return Ok(result);
        }

        // --------------------------------------------------------------------
        // 3️⃣ Category-only search
        // --------------------------------------------------------------------
        [HttpGet("category")]
        public async Task<IActionResult> SearchByCategory([FromQuery] string name)
        {
            var result = await _search.SearchByCategoryAsync(name);
            return Ok(result);
        }

        // --------------------------------------------------------------------
        // 4️⃣ Autocomplete (prefix search)
        // --------------------------------------------------------------------
        [HttpGet("autocomplete")]
        public async Task<IActionResult> AutoComplete([FromQuery] string prefix)
        {
            var result = await _search.AutoCompleteAsync(prefix);
            return Ok(result);
        }

        // --------------------------------------------------------------------
        // 5️⃣ Fuzzy search only (e.g. “aplpe” → “apple”)
        // --------------------------------------------------------------------
        [HttpGet("fuzzy")]
        public async Task<IActionResult> FuzzySearch([FromQuery] string query)
        {
            var result = await _search.FuzzySearchAsync(query);
            return Ok(result);
        }

        // --------------------------------------------------------------------
        // 6️⃣ Combined search (Amazon style)
        // Query + Category + IsDone + Pagination
        // --------------------------------------------------------------------
        [HttpGet("advanced")]
        public async Task<IActionResult> AdvancedSearch(
            [FromQuery] string? query,
            [FromQuery] string? category,
            [FromQuery] bool? isDone,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _search.AdvancedSearchAsync(query, category, isDone, page, pageSize);
            return Ok(result);
        }
    }
}
