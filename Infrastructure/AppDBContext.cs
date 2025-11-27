//using Microsoft.en
using DotNet_Quick_ref_all.Domain;
using Microsoft.EntityFrameworkCore;

namespace DotNet_Quick_ref_all.Infrastructure
{
    public class AppDBContext:DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {
        }
        public DbSet<Domain.TodoItem> TodoItems { get; set; } = null!;
        public DbSet<Domain.Category> Categories { get; set; } = null!;
        /// <summary>
        /// seed initial data
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 2, Name = "Work" },
                new Category { Id = 3, Name = "Home" },
                new Category { Id = 4, Name = "Office" },
                new Category { Id = 5, Name = "Errands" },
                new Category { Id = 6, Name = "Urgent" }
            );

            modelBuilder.Entity<TodoItem>().HasData(
                new TodoItem { Id = 2, Title = "Email boss", IsDone = false, CategoryId = 2 },
                new TodoItem { Id = 3, Title = "Organize desk", IsDone = true, CategoryId = 3 },
                new TodoItem { Id = 4, Title = "Update report", IsDone = false, CategoryId = 4 }
            );
        }

    }
}
