namespace DotNet_Quick_ref_all.DTO
{
    public class TodoSearchResultDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public bool IsDone { get; set; }

        public string? CategoryName { get; set; }

        // We'll add these later when you create Status/Tags tables
        public string? StatusName { get; set; }  // for future
        public List<string> Tags { get; set; } = new();  // for future
    }
}
