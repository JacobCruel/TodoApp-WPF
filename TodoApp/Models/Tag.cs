namespace TodoApp.Models;

public class Tag
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = "#9E9E9E";
    public ICollection<TaskTag> TaskTags { get; set; } = new List<TaskTag>();
}
