using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TodoApp.Models;
using TodoApp.Services;

namespace TodoApp.ViewModels;

public partial class CalendarViewModel : ObservableObject
{
    private readonly TaskService _taskService;
    private List<TodoTask> _allTasks = new();

    [ObservableProperty] private DateTime _currentMonth = new(DateTime.Today.Year, DateTime.Today.Month, 1);
    [ObservableProperty] private string _monthYearText = string.Empty;
    [ObservableProperty] private CalendarDayViewModel? _selectedDay;

    public ObservableCollection<CalendarDayViewModel> Days { get; } = new();

    public CalendarViewModel(TaskService taskService)
    {
        _taskService = taskService;
        UpdateMonthYearText();
    }

    public async Task LoadAsync()
    {
        _allTasks = await _taskService.GetAllTasksAsync();
        BuildCalendarGrid();
    }

    [RelayCommand]
    private void PreviousMonth()
    {
        CurrentMonth = CurrentMonth.AddMonths(-1);
        UpdateMonthYearText();
        BuildCalendarGrid();
    }

    [RelayCommand]
    private void NextMonth()
    {
        CurrentMonth = CurrentMonth.AddMonths(1);
        UpdateMonthYearText();
        BuildCalendarGrid();
    }

    [RelayCommand]
    private void SelectDay(CalendarDayViewModel day)
    {
        if (SelectedDay != null) SelectedDay.IsSelected = false;
        day.IsSelected = true;
        SelectedDay = day;
    }

    private void UpdateMonthYearText()
    {
        MonthYearText = CurrentMonth.ToString("MMMM yyyy");
    }

    private void BuildCalendarGrid()
    {
        Days.Clear();

        var firstDay = CurrentMonth;
        var daysInMonth = DateTime.DaysInMonth(firstDay.Year, firstDay.Month);

        // Pondělí = 0, Neděle = 6 (ISO standard)
        var startDayOfWeek = ((int)firstDay.DayOfWeek + 6) % 7;

        // Dny z předchozího měsíce
        var prevMonth = firstDay.AddMonths(-1);
        var daysInPrevMonth = DateTime.DaysInMonth(prevMonth.Year, prevMonth.Month);
        for (int i = startDayOfWeek - 1; i >= 0; i--)
        {
            var date = new DateTime(prevMonth.Year, prevMonth.Month, daysInPrevMonth - i);
            Days.Add(CreateDayViewModel(date, false));
        }

        // Dny aktuálního měsíce
        for (int day = 1; day <= daysInMonth; day++)
        {
            var date = new DateTime(firstDay.Year, firstDay.Month, day);
            Days.Add(CreateDayViewModel(date, true));
        }

        // Doplnit do 42 (6 řádků × 7 sloupců)
        var nextMonth = firstDay.AddMonths(1);
        int remaining = 42 - Days.Count;
        for (int day = 1; day <= remaining; day++)
        {
            var date = new DateTime(nextMonth.Year, nextMonth.Month, day);
            Days.Add(CreateDayViewModel(date, false));
        }
    }

    private CalendarDayViewModel CreateDayViewModel(DateTime date, bool isCurrentMonth)
    {
        var dayTasks = _allTasks
            .Where(t => t.Deadline?.Date == date.Date)
            .Select(t => new TodoTaskViewModel(t))
            .ToList();

        return new CalendarDayViewModel
        {
            Date = date,
            DayNumber = date.Day,
            IsCurrentMonth = isCurrentMonth,
            IsToday = date.Date == DateTime.Today,
            Tasks = new ObservableCollection<TodoTaskViewModel>(dayTasks)
        };
    }
}

public partial class CalendarDayViewModel : ObservableObject
{
    public DateTime Date { get; set; }
    public int DayNumber { get; set; }

    [ObservableProperty] private bool _isCurrentMonth;
    [ObservableProperty] private bool _isToday;
    [ObservableProperty] private bool _isSelected;

    public ObservableCollection<TodoTaskViewModel> Tasks { get; set; } = new();
    public bool HasTasks => Tasks.Count > 0;
    public int TaskCount => Tasks.Count;
}
