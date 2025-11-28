using DotNet_Quick_ref_all.FuzzySearch;
using Microsoft.AspNetCore.Mvc;

namespace DotNet_Quick_ref_all.Controllers
{
    [ApiController]
    [Route("api/fuzzy")]
    public class FuzzySearchController : ControllerBase
    {
        private readonly FuzzySearchService _service;
        private readonly FuzzySearchServiceDUM _dumService;
        private readonly SmartFuzzySearchService _smartFuzzy;
            //= new SmartFuzzySearchService(new Infrastructure.AppDBContext());

        public FuzzySearchController(FuzzySearchService service,FuzzySearchServiceDUM dumService , SmartFuzzySearchService smartFuzzy)
        {
            _service = service;
            _dumService = dumService;
            _smartFuzzy = smartFuzzy;

        }
        /// <summary>
        /// fuzzy search endpoint
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        [HttpGet("fusearch")]
        public async Task<IActionResult> Search([FromQuery] string q)
        {
            var results = await _service.SearchAsync(q);
            return Ok(results);
        }

        /// <summary>
        /// super advanced fuzzy search with suggestions
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        [HttpGet("fusearchdum")]
        public async Task<IActionResult> dumSearch([FromQuery] string q)
        {
            var results = await _dumService.SearchAsync(q);
            return Ok(results);
        }

        [HttpGet("fusearchdumsuggestions")]
        public async Task<IActionResult> dumSearchSug([FromQuery] string q)
        {
            var results = await _dumService.SearchAsyncsuggestions(q);
            return Ok(results);
        }
        /// <summary>
        ///   get smart fuzzy search results with suggestions
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        [HttpGet("smartsearch")]
        public async Task<IActionResult> SmartSearch([FromQuery] string q)
        {
            var result = await _smartFuzzy.SearchAsync(q);
            return Ok(result);
        }
    }
}
