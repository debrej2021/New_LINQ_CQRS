using DotNet_Quick_ref_all.Dependency_i;
using DotNet_Quick_ref_all.Elastic;
using DotNet_Quick_ref_all.Infrastructure;
using DotNet_Quick_ref_all.OpenAI;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nest;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


namespace DotNet_Quick_ref_all
{
    internal class Program
    {
        /// <summary>
        /// Create → Configure → Build → Run
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            // -------------------------------------------------------
            // 1️⃣ CREATE  
            // -------------------------------------------------------
            // Create the WebApplicationBuilder object.
            // This sets up Hosting, Logging, Configuration, Kestrel.
            // DI container is created here but EMPTY at this stage.
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            // 2️⃣ Register AppDbContext with SQL Server provider
            builder.Services.AddDbContext<AppDBContext>(options =>
                   options.UseSqlServer(connectionString));
            // -------------------------------------------------------
            // 2️⃣ CONFIGURE (Register Services into DI Container)
            // -------------------------------------------------------
            // Add your custom service — whenever Idependent is needed,
            // ASP.NET Core will inject Dependent_imp.
            builder.Services.AddScoped<Idependent, Dependent_imp>();
            builder.Services.AddSingleton<IElasticClient>(
    sp => ElasticClientFactory.Create("http://localhost:9200")
);

            builder.Services.AddScoped<ElasticIndexerService>();
            builder.Services.AddScoped<ElasticSearchService>();

            builder.Services.AddSingleton<OpenAiEmbeddingService>();
            builder.Services.AddSingleton<OpenAiVectorSearchService>();

            // Add controller support (MVC Minimal pipeline)
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            /// Example of Minimal API endpoint (commented out)
            //var app = builder.Build();

            //app.MapGet("/sum", async (Idependent dep) =>
            //{
            //    return await dep.GetTotalSum();
            //});
            // -------------------------------------------------------
            // 3️⃣ BUILD  
            // -------------------------------------------------------
            // This finalizes the DI container and prepares the app object.
            // After this point, you CANNOT register more services.
            var app = builder.Build();

            //if (app.Environment.IsDevelopment())
            //{
            app.UseSwagger();
            app.UseSwaggerUI();
            //  }
            // -------------------------------------------------------
            // 4️⃣ RUN (Configure Middleware + Start Listening)
            // -------------------------------------------------------
            // Map controller endpoints (e.g., /api/xyz)
            app.MapControllers();
            // app.UseMiddleware<ErrorHandlingMiddleware>();
            // Start the web server — this is a blocking call.

            app.MapPost("/seed", async (AppDBContext db) =>
            {
                var item = new Domain.TodoItem
                {
                    Title = "My first Todo!",
                    IsDone = false
                };

                db.TodoItems.Add(item);
                await db.SaveChangesAsync();

                return Results.Ok(item);
            });
            /// Example of seeding data using a scope (commented out)
            //using (var scope = app.Services.CreateScope())
            //{
            //    var db = scope.ServiceProvider.GetRequiredService<AppDBContext>();

            //    if (!db.TodoItems.Any())
            //    {
            //        db.TodoItems.Add(new Domain.TodoItem { Title = "Seed item", IsDone = false });
            //        db.SaveChanges();
            //    }
            //}
            /// Example of changing identity seed using EF Core migration (comments only) seed as well 
            //            Add - Migration ChangeIdentitySeed
            //Update - Database
            /// Example of seeding data using a DbInitializer (commented out)
            //using (var scope = app.Services.CreateScope())
            //{
            //    var db = scope.ServiceProvider.GetRequiredService<AppDBContext>();
            //    await DbInitializer.SeedAsync(db);
            //}


            app.Run();


            // -------------------------------------------------------
            // OPTIONAL (Old code)
            // -------------------------------------------------------
            //Console.WriteLine("Hello, World!");
        }

    }
}
