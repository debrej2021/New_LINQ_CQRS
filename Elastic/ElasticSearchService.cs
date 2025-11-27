using Nest;

namespace DotNet_Quick_ref_all.Elastic
{
    public class ElasticSearchService
    {
        private readonly ElasticClient _client;
        private const string IndexName = "todos";

        public ElasticSearchService(ElasticClient client)
        {
            _client = client;
        }

        // ---------------------------------------------------------
        // 1️⃣ Full text boosted search (Title > Category > FullText)
        // ---------------------------------------------------------
        public async Task<object> SearchAsync(string query, int page = 1, int pageSize = 10)
        {
            var result = await _client.SearchAsync<ElasticTodoDocument>(s => s
                .Index(IndexName)
                .TrackTotalHits(true)
                .From((page - 1) * pageSize)
                .Size(pageSize)
                .Query(q => q
                    .MultiMatch(mm => mm
                        .Query(query)
                        .Fields(f => f
                            .Field(x => x.Title, 4.0)
                            .Field(x => x.CategoryName, 2.0)
                            .Field(x => x.FullText)
                        )
                        .Fuzziness(Fuzziness.Auto)
                    )
                )
            );

            return new
            {
                Total = result.Total,
                Page = page,
                PageSize = pageSize,
                Data = result.Documents
            };
        }

        // ---------------------------------------------------------
        // 2️⃣ Category exact match search
        // ---------------------------------------------------------
        public async Task<object> SearchByCategoryAsync(string category)
        {
            var result = await _client.SearchAsync<ElasticTodoDocument>(s => s
                .Index(IndexName)
                .TrackTotalHits(true)
                .Query(q => q
                    .Term(t => t
                        .Field(f => f.CategoryName.Suffix("keyword"))
                        .Value(category.ToLower())
                    )
                )
            );

            return result.Documents;
        }

        // ---------------------------------------------------------
        // 3️⃣ Autocomplete (prefix search)
        // ---------------------------------------------------------
        public async Task<object> AutoCompleteAsync(string prefix)
        {
            var result = await _client.SearchAsync<ElasticTodoDocument>(s => s
                .Index(IndexName)
                .Size(10)
                .TrackTotalHits(true)
                .Query(q => q
                    .Prefix(p => p
                        .Field(x => x.Title)
                        .Value(prefix.ToLower())
                    )
                )
            );

            return result.Documents.Select(d => d.Title);
        }

        // ---------------------------------------------------------
        // 4️⃣ Fuzzy search (handles typos)
        // ---------------------------------------------------------
        public async Task<object> FuzzySearchAsync(string query)
        {
            var result = await _client.SearchAsync<ElasticTodoDocument>(s => s
                .Index(IndexName)
                .TrackTotalHits(true)
                .Query(q => q
                    .Fuzzy(f => f
                        .Field(x => x.Title)
                        .Value(query)
                        .Fuzziness(Fuzziness.Auto)
                    )
                )
            );

            return result.Documents;
        }

        // ---------------------------------------------------------
        // 5️⃣ Amazon-style combined search:
        //    Full Text + Category + IsDone + Pagination
        // ---------------------------------------------------------
        public async Task<object> AdvancedSearchAsync(
            string? query,
            string? category,
            bool? isDone,
            int page,
            int pageSize)
        {
            var mustQueries = new List<QueryContainer>();

            // Full text search
            if (!string.IsNullOrWhiteSpace(query))
            {
                mustQueries.Add(new MultiMatchQuery
                {
                    Query = query,
                    Fields = new[]
                    {
                        (Field)"title^4",
                        (Field)"categoryName^2",
                        (Field)"fullText"
                    },
                    Fuzziness = Fuzziness.Auto
                });
            }

            // Category filter
            if (!string.IsNullOrWhiteSpace(category))
            {
                mustQueries.Add(new TermQuery
                {
                    Field = "categoryName.keyword",
                    Value = category.ToLower()
                });
            }

            // IsDone filter
            if (isDone.HasValue)
            {
                mustQueries.Add(new TermQuery
                {
                    Field = "isDone",
                    Value = isDone.Value
                });
            }

            var response = await _client.SearchAsync<ElasticTodoDocument>(s => s
                .Index(IndexName)
                .TrackTotalHits(true)
                .From((page - 1) * pageSize)
                .Size(pageSize)
                .Query(q =>
                    mustQueries.Count == 0
                        ? q.MatchAll()
                        : q.Bool(b => b.Must(mustQueries.ToArray()))
                )
            );

            return new
            {
                Total = response.Total,
                Page = page,
                PageSize = pageSize,
                Data = response.Documents
            };
        }
    }
}
