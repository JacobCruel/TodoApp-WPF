using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TodoApp.Models;
using TodoApp.Services;

namespace TodoApp.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly TaskService _taskService;
    private AppSettings? _settings;

    [ObservableProperty] private bool _isDarkTheme = true;
    [ObservableProperty] private string _accentColor = "#2196F3";

    public event Action<bool>? ThemeChanged;
    public event Action<string>? AccentColorChanged;

    public SettingsViewModel(TaskService taskService)
    {
        _taskService = taskService;
    }

    public async Task LoadAsync()
    {
        _settings = await _taskService.GetSettingsAsync();
        IsDarkTheme = _settings.IsDarkTheme;
        AccentColor = _settings.AccentColor;
    }

    partial void OnIsDarkThemeChanged(bool value)
    {
        ThemeChanged?.Invoke(value);
        _ = SaveSettingsAsync();
    }

    partial void OnAccentColorChanged(string value)
    {
        AccentColorChanged?.Invoke(value);
        _ = SaveSettingsAsync();
    }

    private async Task SaveSettingsAsync()
    {
        if (_settings == null) return;
        _settings.IsDarkTheme = IsDarkTheme;
        _settings.AccentColor = AccentColor;
        await _taskService.SaveSettingsAsync(_settings);
    }

    [RelayCommand]
    private void SetAccentColor(string color)
    {
        AccentColor = color;
    }
}
