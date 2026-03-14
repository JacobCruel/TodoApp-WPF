namespace TodoApp.Models;

public class TaskTag
{
    public int TodoTaskId { get; set; }
    public TodoTask TodoTask { get; set; } = null!;
    public int TagId { get; set; }
    public Tag Tag { get; set; } = null!;
}
