namespace DotNet_Quick_ref_all.Elastic
{
    /// <summary>
    /// elasticsearch document for Todo item
    /// </summary>
    public class ElasticTodoDocument
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public bool IsDone { get; set; }
        public string CategoryName { get; set; } = string.Empty;

        public string? FullText { get; set; }

    }
}
