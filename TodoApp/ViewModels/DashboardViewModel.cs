using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using TodoApp.Services;

namespace TodoApp.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    private readonly TaskService _taskService;

    [ObservableProperty] private int _totalTasks;
    [ObservableProperty] private int _completedTasks;
    [ObservableProperty] private int _overdueTasks;
    [ObservableProperty] private int _dueThisWeek;
    [ObservableProperty] private double _completionRate;

    public ObservableCollection<TodoTaskViewModel> UpcomingTasks { get; } = new();

    public DashboardViewModel(TaskService taskService)
    {
        _taskService = taskService;
    }

    public async Task LoadAsync()
    {
        var tasks = await _taskService.GetAllTasksAsync();

        TotalTasks = tasks.Count;
        CompletedTasks = tasks.Count(t => t.IsCompleted);
        OverdueTasks = tasks.Count(t => !t.IsCompleted && t.Deadline.HasValue && t.Deadline.Value.Date < DateTime.Today);
        DueThisWeek = tasks.Count(t => !t.IsCompleted && t.Deadline.HasValue &&
            t.Deadline.Value.Date >= DateTime.Today &&
            t.Deadline.Value.Date <= DateTime.Today.AddDays(7));
        CompletionRate = TotalTasks > 0 ? (double)CompletedTasks / TotalTasks * 100 : 0;

        UpcomingTasks.Clear();
        foreach (var task in tasks
            .Where(t => !t.IsCompleted && t.Deadline.HasValue)
            .OrderBy(t => t.Deadline)
            .Take(5))
        {
            UpcomingTasks.Add(new TodoTaskViewModel(task));
        }
    }
}
