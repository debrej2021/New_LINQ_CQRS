using DotNet_Quick_ref_all.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DotNet_Quick_ref_all.DTO;
using DotNet_Quick_ref_all.Domain;
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
                }).Distinct()
                .ToListAsync();

            //var resultInner = await _db.TodoItems
            //           .Where(t => t.Category != null)
            //           .Select(t => new TodoSearchResultDto
            //           {
            //               Id = t.Id,
            //               Title = t.Title,
            //               IsDone = t.IsDone,
            //               CategoryName = t.Category.Name
            //           }).ToListAsync();

            return Ok(result);
        }

        [HttpGet("inner-join")]
        public async Task<IActionResult> InnerJoin()
        {
            var resultInner = await _db.TodoItems
                       .Where(t => t.Category != null)
                       .Select(t => new TodoSearchResultDto
                       {
                           Id = t.Id,
                           Title = t.Title,
                           IsDone = t.IsDone,
                           CategoryName = t.Category.Name
                       }).ToListAsync();

            return Ok(resultInner);
        }
        /// <summary>
        /// cross join example
        /// </summary>
        /// <returns></returns>
        [HttpGet("cross-join")]
        public async Task<IActionResult> CrossJoin()
        {
            var resCat = _db.Categories
                        .Select(c => c.Name)
                        .Distinct();
            //var resultInner = await _db.TodoItems
            //           .Where(t => t.Category != null)
            //           .Select(t => new TodoSearchResultDto
            //           {
            //               Id = t.Id,
            //               Title = t.Title,
            //               IsDone = t.IsDone,
            //               CategoryName = t.Category.Name
            //           }).ToListAsync();

            var result =
    await (from c in resCat          // c = string (category name)
           from t in _db.TodoItems   // t = TodoItem
           select new
           {
               CategoryName = c,
               TodoTitle = t.Title
           })
    .ToListAsync();


            return Ok(result);
        }

        /// <summary>
        /// Retrieves a distinct list of category and to-do item title pairs.
        /// </summary>
        /// <remarks>This method queries the database for to-do items that are associated with a category,
        /// and returns a collection of unique pairs consisting of the category name and the to-do item title.</remarks>
        /// <returns>An <see cref="IActionResult"/> containing a collection of unique category and to-do item title pairs. Each
        /// pair includes the category name and the to-do item title.</returns>
        [HttpGet("categories-todos/distinct")]
        public async Task<IActionResult> GetDistinctCategoryTodoPairs()
        {
            var result = await _db.TodoItems
                .Include(t => t.Category)
                .Where(t => t.Category != null)
                .Select(t => new
                {
                    CategoryName = t.Category!.Name,
                    TodoTitle = t.Title
                })
                .Distinct()
                .ToListAsync();

            return Ok(result);
        }
    }
}
