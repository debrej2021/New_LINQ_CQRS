using DotNet_Quick_ref_all.Domain;

namespace DotNet_Quick_ref_all.Infrastructure
{
    public static class DbInitializer
    {
        /// <summary>
        /// seed initial data into the database
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public static async Task SeedAsync(AppDBContext db)
        {
            // 🔸 Ensure database is created (safe)
            await db.Database.EnsureCreatedAsync();

            // --------------------------------------------
            // 1️⃣ SEED CATEGORIES IF EMPTY
            // --------------------------------------------
            if (!db.Categories.Any())
            {
                db.Categories.AddRange(
                    new Category { Name = "Work" },
                    new Category { Name = "Home" },
                    new Category { Name = "Office" },
                    new Category { Name = "Errands" },
                    new Category { Name = "Urgent" }
                );

                await db.SaveChangesAsync();
            }

            // --------------------------------------------
            // 2️⃣ SEED TODO ITEMS IF EMPTY
            // --------------------------------------------
            if (!db.TodoItems.Any())
            {
                // Get some category IDs to link FK
                var workId = db.Categories.First(c => c.Name == "Work").Id;
                var homeId = db.Categories.First(c => c.Name == "Home").Id;

                db.TodoItems.AddRange(
                    new TodoItem
                    {
                        Title = "Email boss",
                        IsDone = false,
                        CategoryId = workId
                    },

                    new TodoItem
                    {
                        Title = "Organize desk",
                        IsDone = true,
                        CategoryId = workId
                    },

                    new TodoItem
                    {
                        Title = "Clean kitchen",
                        IsDone = false,
                        CategoryId = homeId
                    }
                );

                await db.SaveChangesAsync();
            }

            // --------------------------------------------
            // 3️⃣ OPTIONAL: MORE SEEDING LOGIC LATER
            // --------------------------------------------
        }
    }
}
