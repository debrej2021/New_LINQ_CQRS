using DotNet_Quick_ref_all.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Nest;

namespace DotNet_Quick_ref_all.Elastic
{
    public class ElasticIndexerService
    {
        private readonly AppDBContext _db;
        private readonly ElasticClient _client;

        private const string IndexName = "todos";

        public ElasticIndexerService(AppDBContext db, ElasticClient client)
        {
            _db = db;
            _client = client;
        }

        // ---------------------------------------------------------
        // 1️⃣ Ensure index exists (auto-map fields)
        // ---------------------------------------------------------
        private async Task CreateIndexAsync()
        {
            await _client.Indices.CreateAsync(IndexName, c => c
                .Map<ElasticTodoDocument>(m => m.AutoMap())
            );
        }

        // ---------------------------------------------------------
        // 2️⃣ Reindex all DB → Elasticsearch (Full rebuild)
        // ---------------------------------------------------------
        public async Task ReindexAsync()
        {
            // Delete existing index
            var exists = await _client.Indices.ExistsAsync(IndexName);
            if (exists.Exists)
                await _client.Indices.DeleteAsync(IndexName);

            // Recreate
            await CreateIndexAsync();

            // Fetch DB data
            var items = await _db.TodoItems
                .Include(t => t.Category)
                .ToListAsync();

            // Convert DB → ES document model
            var documents = items.Select(t => new ElasticTodoDocument
            {
                Id = t.Id,
                Title = t.Title,
                IsDone = t.IsDone,
                CategoryName = t.Category?.Name?.ToLower() ?? "",
                FullText = $"{t.Title} {t.Category?.Name}".ToLower()
            });

            // Bulk index
            var response = await _client.BulkAsync(b => b
                .Index(IndexName)
                .IndexMany(documents)
                .Refresh(Elasticsearch.Net.Refresh.True)
            );

            if (response.Errors)
            {
                throw new Exception("Elastic bulk indexing failed: " +
                    string.Join(" | ", response.ItemsWithErrors.Select(e => e.Error.Reason)));
            }
        }

        // ---------------------------------------------------------
        // 3️⃣ Index a single item (create/update)
        // ---------------------------------------------------------
        public async Task IndexItemAsync(int id)
        {
            var t = await _db.TodoItems
                .Include(c => c.Category)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (t == null) return;

            var doc = new ElasticTodoDocument
            {
                Id = t.Id,
                Title = t.Title,
                IsDone = t.IsDone,
                CategoryName = t.Category?.Name?.ToLower() ?? "",
                FullText = $"{t.Title} {t.Category?.Name}".ToLower()
            };

            await _client.IndexAsync(doc, i => i.Index(IndexName).Id(doc.Id));
        }
    }
}
