namespace TodoApp.Models;

public class SubTask
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public int Order { get; set; }
    public int TodoTaskId { get; set; }
    public TodoTask TodoTask { get; set; } = null!;
}
