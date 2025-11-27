using DotNet_Quick_ref_all.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DotNet_Quick_ref_all.DTO;
namespace DotNet_Quick_ref_all.Controllers
{
    [Route("api/search")]
    [ApiController]
    public class SearchController : Controller
    {
        private readonly AppDBContext _db;

        public SearchController(AppDBContext db)
        {
            _db = db;
        }
        /// <summary>
        /// get todo items with titles starting with a vowel, with pagination
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        // GET api/search/vowels?page=1&pageSize=5
        [HttpGet("vowels")]
        public async Task<IActionResult> GetVowelTodos(int page = 1, int pageSize = 5)
        {
            char[] vowels = { 'A', 'E', 'I', 'O', 'U' };

            // Query stays IQueryable = SQL side execution
            var query = _db.TodoItems
                .Include(t => t.Category)
                .Where(t =>
                    !string.IsNullOrEmpty(t.Title) &&
                    vowels.Contains(char.ToUpper(t.Title[0])));

            // Pagination
            var totalRecords = await query.CountAsync();

            var result = await query
                .OrderBy(t => t.Title)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new
                {
                    t.Id,
                    t.Title,
                    Category = t.Category!.Name
                })
                .ToListAsync();

            return Ok(new
            {
                Page = page,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize),
                Data = result
            });
        }
        /// <summary>
        /// complex Join
        /// </summary>
        /// <returns></returns>
        // GET api/search/complex-join
        [HttpGet("complex-join")]
        public async Task<IActionResult> ComplexJoin()
        {
            // Base query with JOIN to Category
            var query = _db.TodoItems
                .Include(t => t.Category)
                .AsQueryable();

            // Project directly to DTO from SQL
            var result = await query
                .Select(t => new TodoSearchResultDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    IsDone = t.IsDone,
                    CategoryName = t.Category != null ? t.Category.Name : null,

                    // These will just be empty for now
                    StatusName = null,
                    Tags = new List<string>()
                })
                .ToListAsync();

            return Ok(result);
        }


    }
}
