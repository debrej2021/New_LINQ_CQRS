using DotNet_Quick_ref_all.Domain;
using DotNet_Quick_ref_all.DTO;
using DotNet_Quick_ref_all.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using static DotNet_Quick_ref_all.DTO.ToDoItemDto;

namespace DotNet_Quick_ref_all.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TodoItemController : ControllerBase
    {
        private readonly AppDBContext _db;

        public TodoItemController(AppDBContext db)
        {
            _db = db;
        }
        // GET /Todo/with-vowels
        [HttpGet("with-vowels")]
        public async Task<IActionResult> GetVowelItems()
        {
            char[] vowels = { 'A', 'E', 'I', 'O', 'U' };

            // 1️⃣ First part runs fully in SQL (Entity Framework)
            var raw = await _db.TodoItems
                .Include(t => t.Category)
                .Where(t =>
                    !string.IsNullOrEmpty(t.Title) &&
                    vowels.Contains(char.ToUpper(t.Title[0])))
                .Select(t => new
                {
                    t.Id,
                    t.Title,
                    CategoryName = t.Category!.Name
                })
                .ToListAsync(); // 👈 This part is async and must be awaited

            // 2️⃣ Now convert anonymous type → tuple (in memory)
            var result = raw
                .Select(x => (x.Id, x.Title, x.CategoryName))
                .ToList();

            return Ok(result);
        }
        // GET /Todo
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItemDto>>> GetAll()
        {
            var items = await _db.TodoItems
                .Select(t => new TodoItemDto
                {
                    Id = t.Id,
                    Title = t.Title,
                    IsDone = t.IsDone
                })
                .ToListAsync();

            return Ok(items);
        }

        // GET /Todo/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<TodoItemDto>> GetById(int id)
        {
            var item = await _db.TodoItems.FindAsync(id);
            if (item == null) return NotFound();

            return Ok(new TodoItemDto
            {
                Id = item.Id,
                Title = item.Title,
                IsDone = item.IsDone
            });
        }

        // POST /Todo
        [HttpPost]
        public async Task<ActionResult<TodoItemDto>> Create(CreateTodoDto dto)
        {
            var todo = new TodoItem
            {
                Title = dto.Title,
                IsDone = false
            };

            _db.TodoItems.Add(todo);
            await _db.SaveChangesAsync();

            var result = new TodoItemDto
            {
                Id = todo.Id,
                Title = todo.Title,
                IsDone = todo.IsDone
            };

            return CreatedAtAction(nameof(GetById), new { id = todo.Id }, result);
        }

        // PUT /Todo/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, UpdateTodoDto dto)
        {
            var todo = await _db.TodoItems.FindAsync(id);
            if (todo == null) return NotFound();

            todo.Title = dto.Title;
            todo.IsDone = dto.IsDone;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        // DELETE /Todo/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var todo = await _db.TodoItems.FindAsync(id);
            if (todo == null) return NotFound();

            _db.TodoItems.Remove(todo);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
