namespace TodoApp.Models;

public class TodoTask
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTime? Deadline { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? CompletedAt { get; set; }

    public Priority Priority { get; set; } = Priority.Medium;
    public RecurrenceType Recurrence { get; set; } = RecurrenceType.None;

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public ICollection<TaskTag> TaskTags { get; set; } = new List<TaskTag>();
    public ICollection<SubTask> SubTasks { get; set; } = new List<SubTask>();
}
