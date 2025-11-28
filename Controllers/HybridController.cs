using DotNet_Quick_ref_all.HybridSearch;
using Microsoft.AspNetCore.Mvc;

namespace DotNet_Quick_ref_all.Controllers
{
    public class HybridController:ControllerBase
    {
        private readonly HybridSearchService _hybridSearch;

        public HybridController(HybridSearchService hybrid)
        {
            _hybridSearch = hybrid;
        }
        /// <summary>
        /// hybrid search endpoint
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        [HttpGet("hybrid")]
        public async Task<IActionResult> Hybrid([FromQuery] string q)
        {
            var result = await _hybridSearch.SearchAsync(q);
            return Ok(result);
        }

    }
}
