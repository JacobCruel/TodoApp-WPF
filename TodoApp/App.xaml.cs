using System.Windows;
using System.Windows.Media;
using TodoApp.Services;
using TodoApp.ViewModels;
using TodoApp.Views;

namespace TodoApp;

public partial class App : Application
{
    private string _currentAccentColor = "#E94560";

    private async void OnStartup(object sender, StartupEventArgs e)
    {
        // Migrace starých dat
        var migrationService = new DataMigrationService();
        await migrationService.MigrateIfNeededAsync();

        var taskService = new TaskService();
        var mainViewModel = new MainViewModel(taskService);

        // Načtení settings pro theme
        var settings = await taskService.GetSettingsAsync();

        // Nastavení theme + accent
        _currentAccentColor = settings.AccentColor;
        SetTheme(settings.IsDarkTheme);
        SetAccentColor(settings.AccentColor);

        // Propojení settings s theme a accent
        mainViewModel.SettingsViewModel.ThemeChanged += isDark =>
        {
            SetTheme(isDark);
            // Re-apply accent color after theme switch, because the new theme
            // dictionary contains default AccentBrush values that can interfere
            SetAccentColor(_currentAccentColor);
        };
        mainViewModel.SettingsViewModel.AccentColorChanged += color => SetAccentColor(color);

        var mainWindow = new MainWindow
        {
            DataContext = mainViewModel
        };
        mainWindow.Show();

        // Inicializace dat
        await mainViewModel.SettingsViewModel.LoadAsync();
        await mainViewModel.InitializeAsync();
    }

    public void SetTheme(bool isDark)
    {
        var themePath = isDark
            ? "Resources/Themes/DarkTheme.xaml"
            : "Resources/Themes/LightTheme.xaml";

        var themeDict = new ResourceDictionary { Source = new Uri(themePath, UriKind.Relative) };

        // Nahradíme první ResourceDictionary (theme) v MergedDictionaries
        if (Resources.MergedDictionaries.Count > 0)
            Resources.MergedDictionaries[0] = themeDict;
    }

    public void SetAccentColor(string hex)
    {
        try
        {
            var color = (Color)ColorConverter.ConvertFromString(hex);
            var hoverColor = LightenColor(color, 0.2);
            var pressedColor = DarkenColor(color, 0.2);

            _currentAccentColor = hex;

            Resources["AccentBrush"] = new SolidColorBrush(color);
            Resources["AccentHoverBrush"] = new SolidColorBrush(hoverColor);
            Resources["AccentPressedBrush"] = new SolidColorBrush(pressedColor);
            Resources["InputFocusBorderBrush"] = new SolidColorBrush(color);
        }
        catch
        {
            // Invalid color string — ignore
        }
    }

    private static Color LightenColor(Color color, double factor)
    {
        return Color.FromArgb(color.A,
            (byte)Math.Min(255, color.R + (255 - color.R) * factor),
            (byte)Math.Min(255, color.G + (255 - color.G) * factor),
            (byte)Math.Min(255, color.B + (255 - color.B) * factor));
    }

    private static Color DarkenColor(Color color, double factor)
    {
        return Color.FromArgb(color.A,
            (byte)(color.R * (1 - factor)),
            (byte)(color.G * (1 - factor)),
            (byte)(color.B * (1 - factor)));
    }
}
