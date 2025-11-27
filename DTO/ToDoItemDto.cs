using System;
namespace DotNet_Quick_ref_all.DTO
{
    public class ToDoItemDto
    {
        // Used when creating a new Todo
        public class CreateTodoDto
        {
            public string Title { get; set; } = string.Empty;
        }

        // Used when updating an existing Todo
        public class UpdateTodoDto
        {
            public string Title { get; set; } = string.Empty;
            public bool IsDone { get; set; }
        }

        // What we send back to clients
        public class TodoItemDto
        {
            public int Id { get; set; }
            public string Title { get; set; } = string.Empty;
            public bool IsDone { get; set; }
        }
    }
}
