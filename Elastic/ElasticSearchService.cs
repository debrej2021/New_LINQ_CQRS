using Nest;

namespace DotNet_Quick_ref_all.Elastic
{
    /// <summary>
    /// elasticsearch search service
    /// </summary>
    public class ElasticSearchService
    {
        private readonly IElasticClient _client;

        public ElasticSearchService(IElasticClient client)
        {
            _client = client;
        }

        // Full text search
        public async Task<object> SearchAsync(string query)
        {
            var result = await _client.SearchAsync<ElasticTodoDocument>(s => s
                .Query(q => q
                    .MultiMatch(mm => mm
                        .Query(query)
                        .Fields(f => f
                            .Field(x => x.Title)
                            .Field(x => x.CategoryName)
                        )
                    )
                )
            );

            return result.Documents;
        }

        // Filter by category only
        public async Task<object> SearchByCategoryAsync(string category)
        {
            var result = await _client.SearchAsync<ElasticTodoDocument>(s => s
                .Query(q => q
                    .Term(t => t.Field(x => x.CategoryName).Value(category.ToLower()))
                )
            );

            return result.Documents;
        }
    }
}
