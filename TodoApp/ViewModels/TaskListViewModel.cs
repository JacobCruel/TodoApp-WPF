using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TodoApp.Models;
using TodoApp.Services;

namespace TodoApp.ViewModels;

public partial class TaskListViewModel : ObservableObject
{
    private readonly TaskService _taskService;
    private readonly UndoRedoService? _undoRedoService;
    private readonly Action<string, string>? _showToast;

    public ObservableCollection<TodoTaskViewModel> Tasks { get; } = new();
    public ObservableCollection<Category> Categories { get; } = new();

    private ICollectionView? _tasksView;
    public ICollectionView TasksView => _tasksView ??= CollectionViewSource.GetDefaultView(Tasks);

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private Category? _selectedCategoryFilter;

    [ObservableProperty]
    private string _sortBy = "Deadline";

    [ObservableProperty]
    private bool _showCompleted = true;

    [ObservableProperty]
    private bool _isDialogOpen;

    [ObservableProperty]
    private TodoTaskViewModel? _editingTask;

    [ObservableProperty]
    private bool _isEditMode;

    private int _editingOriginalId;

    public TaskListViewModel(TaskService taskService, UndoRedoService? undoRedoService = null, Action<string, string>? showToast = null)
    {
        _taskService = taskService;
        _undoRedoService = undoRedoService;
        _showToast = showToast;
    }

    public async Task LoadTasksAsync()
    {
        var tasks = await _taskService.GetAllTasksAsync();
        var categories = await _taskService.GetCategoriesAsync();

        Tasks.Clear();
        foreach (var task in tasks)
            Tasks.Add(new TodoTaskViewModel(task));

        Categories.Clear();
        foreach (var cat in categories)
            Categories.Add(cat);

        SetupFiltering();
    }

    private void SetupFiltering()
    {
        _tasksView = CollectionViewSource.GetDefaultView(Tasks);
        if (_tasksView != null)
        {
            _tasksView.Filter = FilterTask;
            ApplySorting();
        }
        OnPropertyChanged(nameof(TasksView));
    }

    private bool FilterTask(object obj)
    {
        if (obj is not TodoTaskViewModel task) return false;

        if (!ShowCompleted && task.IsCompleted) return false;

        if (SelectedCategoryFilter != null && task.CategoryId != SelectedCategoryFilter.Id)
            return false;

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            return task.Title.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                   (task.Notes?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                   task.CategoryName.Contains(SearchText, StringComparison.OrdinalIgnoreCase);
        }

        return true;
    }

    partial void OnSearchTextChanged(string value) => TasksView?.Refresh();
    partial void OnSelectedCategoryFilterChanged(Category? value) => TasksView?.Refresh();
    partial void OnShowCompletedChanged(bool value) => TasksView?.Refresh();
    partial void OnSortByChanged(string value) => ApplySorting();

    private void ApplySorting()
    {
        if (TasksView is null) return;
        using (TasksView.DeferRefresh())
        {
            TasksView.SortDescriptions.Clear();
            TasksView.SortDescriptions.Add(SortBy switch
            {
                "Priority" => new SortDescription(nameof(TodoTaskViewModel.Priority), ListSortDirection.Descending),
                "Category" => new SortDescription(nameof(TodoTaskViewModel.CategoryName), ListSortDirection.Ascending),
                "Title" => new SortDescription(nameof(TodoTaskViewModel.Title), ListSortDirection.Ascending),
                "Created" => new SortDescription(nameof(TodoTaskViewModel.CreatedAt), ListSortDirection.Descending),
                _ => new SortDescription(nameof(TodoTaskViewModel.Deadline), ListSortDirection.Ascending),
            });
        }
    }

    [RelayCommand]
    private void OpenAddDialog()
    {
        OpenAddDialogWithDate(DateTime.Today.AddDays(1));
    }

    public void OpenAddDialogWithDate(DateTime deadline)
    {
        EditingTask = new TodoTaskViewModel
        {
            Deadline = deadline,
            Priority = Priority.Medium,
            CategoryId = Categories.FirstOrDefault()?.Id ?? 1
        };
        IsEditMode = false;
        IsDialogOpen = true;
    }

    [RelayCommand]
    private void OpenEditDialog(TodoTaskViewModel task)
    {
        EditingTask = new TodoTaskViewModel
        {
            Title = task.Title,
            Notes = task.Notes,
            Deadline = task.Deadline,
            Priority = task.Priority,
            CategoryId = task.CategoryId,
            Recurrence = task.Recurrence,
            CategoryName = task.CategoryName,
            CategoryColor = task.CategoryColor
        };
        _editingOriginalId = task.Id;
        IsEditMode = true;
        IsDialogOpen = true;
    }

    [RelayCommand]
    private async Task SaveTask()
    {
        if (EditingTask == null || string.IsNullOrWhiteSpace(EditingTask.Title)) return;

        if (IsEditMode)
        {
            var model = EditingTask.ToModel();
            model.Id = _editingOriginalId;
            var original = Tasks.FirstOrDefault(t => t.Id == _editingOriginalId);
            if (original != null)
                model.CreatedAt = original.CreatedAt;
            await _taskService.UpdateTaskAsync(model);
            _showToast?.Invoke("Task updated", "#4CAF50");
        }
        else
        {
            var model = EditingTask.ToModel();
            model.CreatedAt = DateTime.Now;

            if (_undoRedoService != null)
            {
                await _undoRedoService.ExecuteAsync(new CreateTaskAction(_taskService, model));
            }
            else
            {
                await _taskService.CreateTaskAsync(model);
            }
            _showToast?.Invoke($"Task '{model.Title}' added", "#4CAF50");
        }

        IsDialogOpen = false;
        EditingTask = null;
        await LoadTasksAsync();
    }

    [RelayCommand]
    private void CancelDialog()
    {
        IsDialogOpen = false;
        EditingTask = null;
    }

    [RelayCommand]
    private async Task DeleteTask(TodoTaskViewModel task)
    {
        if (_undoRedoService != null)
        {
            var model = task.ToModel();
            model.CategoryId = task.CategoryId;
            await _undoRedoService.ExecuteAsync(new DeleteTaskAction(_taskService, model));
        }
        else
        {
            await _taskService.DeleteTaskAsync(task.Id);
        }

        Tasks.Remove(task);
        _showToast?.Invoke($"Task '{task.Title}' deleted", "#F44336");
    }

    [RelayCommand]
    private async Task ToggleComplete(TodoTaskViewModel task)
    {
        if (_undoRedoService != null)
        {
            await _undoRedoService.ExecuteAsync(new ToggleCompleteAction(_taskService, task.Id, task.Title));
        }
        else
        {
            await _taskService.ToggleCompleteAsync(task.Id);
        }
        await LoadTasksAsync();
    }

    [RelayCommand]
    private void ClearCategoryFilter()
    {
        SelectedCategoryFilter = null;
    }
}
