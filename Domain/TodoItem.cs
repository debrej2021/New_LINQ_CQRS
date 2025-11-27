namespace DotNet_Quick_ref_all.Domain
{
    public class TodoItem
    {
    //    namespace DotNet_Quick_ref_all.Domain
    //{
    //    public class TodoItem
    //    {
            public int Id { get; set; }          // PK
            public string Title { get; set; } = string.Empty;
            public bool IsDone { get; set; }


        // FK
        public int? CategoryId { get; set; }

        // Navigation property
        public Category Category { get; set; } = null!;
    }
    }

//}
//}
