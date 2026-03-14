using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using TodoApp.Models;

namespace TodoApp.ViewModels;

public partial class TodoTaskViewModel : ObservableObject
{
    public int Id { get; }

    [ObservableProperty] private string _title = string.Empty;
    [ObservableProperty] private string? _notes;
    [ObservableProperty] private DateTime? _deadline;
    [ObservableProperty] private bool _isCompleted;
    [ObservableProperty] private DateTime _createdAt;
    [ObservableProperty] private DateTime? _completedAt;
    [ObservableProperty] private Priority _priority;
    [ObservableProperty] private RecurrenceType _recurrence;
    [ObservableProperty] private int _categoryId;
    [ObservableProperty] private string _categoryName = string.Empty;
    [ObservableProperty] private string _categoryColor = "#9E9E9E";

    public ObservableCollection<SubTaskViewModel> SubTasks { get; } = new();
    public ObservableCollection<TagViewModel> Tags { get; } = new();

    public string DeadlineText => Deadline switch
    {
        null => "",
        var d when d.Value.Date == DateTime.Today => "Today",
        var d when d.Value.Date == DateTime.Today.AddDays(1) => "Tomorrow",
        var d when d.Value.Date == DateTime.Today.AddDays(-1) => "Yesterday",
        var d when d.Value.Date < DateTime.Today => $"Overdue {(DateTime.Today - d.Value.Date).Days}d",
        var d when (d.Value.Date - DateTime.Today).Days <= 7 => $"In {(d.Value.Date - DateTime.Today).Days}d",
        var d => d.Value.ToString("d MMM yyyy")
    };

    public bool IsOverdue => !IsCompleted && Deadline.HasValue && Deadline.Value.Date < DateTime.Today;

    public TodoTaskViewModel() { }

    public TodoTaskViewModel(TodoTask task)
    {
        Id = task.Id;
        Title = task.Title;
        Notes = task.Notes;
        Deadline = task.Deadline;
        IsCompleted = task.IsCompleted;
        CreatedAt = task.CreatedAt;
        CompletedAt = task.CompletedAt;
        Priority = task.Priority;
        Recurrence = task.Recurrence;
        CategoryId = task.CategoryId;

        if (task.Category != null)
        {
            CategoryName = task.Category.Name;
            CategoryColor = task.Category.Color;
        }

        if (task.SubTasks != null)
        {
            foreach (var sub in task.SubTasks.OrderBy(s => s.Order))
                SubTasks.Add(new SubTaskViewModel(sub));
        }

        if (task.TaskTags != null)
        {
            foreach (var tt in task.TaskTags)
            {
                if (tt.Tag != null)
                    Tags.Add(new TagViewModel(tt.Tag));
            }
        }
    }

    public TodoTask ToModel() => new()
    {
        Id = Id,
        Title = Title,
        Notes = Notes,
        Deadline = Deadline,
        IsCompleted = IsCompleted,
        CreatedAt = CreatedAt,
        CompletedAt = CompletedAt,
        Priority = Priority,
        Recurrence = Recurrence,
        CategoryId = CategoryId
    };
}

public partial class SubTaskViewModel : ObservableObject
{
    public int Id { get; }

    [ObservableProperty] private string _title = string.Empty;
    [ObservableProperty] private bool _isCompleted;
    [ObservableProperty] private int _order;

    public SubTaskViewModel() { }

    public SubTaskViewModel(SubTask sub)
    {
        Id = sub.Id;
        Title = sub.Title;
        IsCompleted = sub.IsCompleted;
        Order = sub.Order;
    }
}

public partial class TagViewModel : ObservableObject
{
    public int Id { get; }
    [ObservableProperty] private string _name = string.Empty;
    [ObservableProperty] private string _color = "#9E9E9E";

    public TagViewModel() { }

    public TagViewModel(Tag tag)
    {
        Id = tag.Id;
        Name = tag.Name;
        Color = tag.Color;
    }
}
