namespace TodoApp.Models;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = "#2196F3";
    public ICollection<TodoTask> Tasks { get; set; } = new List<TodoTask>();

    public override string ToString() => Name;
}
