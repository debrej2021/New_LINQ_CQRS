using Elasticsearch.Net;
using Nest;

namespace DotNet_Quick_ref_all.Elastic
{
    /// <summary>
    /// Elasticsearch client factory
    /// </summary>
    public static class ElasticClientFactory
    {
        public static IElasticClient Create(string url)
        {
            var settings = new ConnectionSettings(new Uri(url))
                .DefaultIndex("todo-index")
                .PrettyJson()
                .ThrowExceptions();

            return new ElasticClient(settings);
        }
    }
}
