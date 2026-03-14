using System.Collections.ObjectModel;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using TodoApp.Services;

namespace TodoApp.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly TaskService _taskService;
    private readonly UndoRedoService _undoRedoService;
    private readonly ExportService _exportService;

    [ObservableProperty]
    private object? _currentView;

    [ObservableProperty]
    private int _selectedNavIndex;

    public TaskListViewModel TaskListViewModel { get; }
    public DashboardViewModel DashboardViewModel { get; }
    public CalendarViewModel CalendarViewModel { get; }
    public SettingsViewModel SettingsViewModel { get; }

    public ObservableCollection<ToastMessage> Toasts { get; } = new();

    public MainViewModel(TaskService taskService)
    {
        _taskService = taskService;
        _undoRedoService = new UndoRedoService();
        _exportService = new ExportService();

        TaskListViewModel = new TaskListViewModel(taskService, _undoRedoService, ShowToast);
        DashboardViewModel = new DashboardViewModel(taskService);
        CalendarViewModel = new CalendarViewModel(taskService);
        SettingsViewModel = new SettingsViewModel(taskService);

        _currentView = TaskListViewModel;
        _selectedNavIndex = 1;
    }

    public async Task InitializeAsync()
    {
        await TaskListViewModel.LoadTasksAsync();
        await DashboardViewModel.LoadAsync();
    }

    [RelayCommand]
    private async Task NavigateToDashboard()
    {
        SelectedNavIndex = 0;
        await DashboardViewModel.LoadAsync();
        CurrentView = DashboardViewModel;
    }

    [RelayCommand]
    private void NavigateToTasks()
    {
        SelectedNavIndex = 1;
        CurrentView = TaskListViewModel;
    }

    [RelayCommand]
    private async Task NavigateToCalendar()
    {
        SelectedNavIndex = 2;
        await CalendarViewModel.LoadAsync();
        CurrentView = CalendarViewModel;
    }

    [RelayCommand]
    private void NavigateToSettings()
    {
        SelectedNavIndex = 3;
        CurrentView = SettingsViewModel;
    }

    [RelayCommand]
    private void AddTaskShortcut()
    {
        CurrentView = TaskListViewModel;
        SelectedNavIndex = 1;
        TaskListViewModel.OpenAddDialogCommand.Execute(null);
    }

    public void AddTaskForDate(DateTime date)
    {
        CurrentView = TaskListViewModel;
        SelectedNavIndex = 1;
        TaskListViewModel.OpenAddDialogWithDate(date);
    }

    [RelayCommand]
    private async Task Undo()
    {
        if (_undoRedoService.CanUndo)
        {
            var desc = _undoRedoService.UndoDescription;
            await _undoRedoService.UndoAsync();
            await TaskListViewModel.LoadTasksAsync();
            ShowToast($"Undone: {desc}", "#2196F3");
        }
    }

    [RelayCommand]
    private async Task Redo()
    {
        if (_undoRedoService.CanRedo)
        {
            var desc = _undoRedoService.RedoDescription;
            await _undoRedoService.RedoAsync();
            await TaskListViewModel.LoadTasksAsync();
            ShowToast($"Redone: {desc}", "#2196F3");
        }
    }

    [RelayCommand]
    private void ToggleTheme()
    {
        SettingsViewModel.IsDarkTheme = !SettingsViewModel.IsDarkTheme;
    }

    [RelayCommand]
    private async Task Export()
    {
        var dialog = new SaveFileDialog
        {
            Filter = "CSV files (*.csv)|*.csv",
            DefaultExt = ".csv",
            FileName = $"tasks_{DateTime.Now:yyyyMMdd}"
        };

        if (dialog.ShowDialog() == true)
        {
            var tasks = await _taskService.GetAllTasksAsync();
            await _exportService.ExportToCsvAsync(tasks, dialog.FileName);
            ShowToast("Tasks exported successfully", "#4CAF50");
        }
    }

    private async void ShowToast(string message, string color)
    {
        var toast = new ToastMessage { Message = message, Color = new SolidColorBrush(
            (Color)ColorConverter.ConvertFromString(color)) };
        Toasts.Add(toast);

        await Task.Delay(3000);
        Toasts.Remove(toast);
    }
}

public class ToastMessage
{
    public string Message { get; set; } = string.Empty;
    public SolidColorBrush Color { get; set; } = new(Colors.Blue);
}
