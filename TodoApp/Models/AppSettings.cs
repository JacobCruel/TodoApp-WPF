namespace TodoApp.Models;

public class AppSettings
{
    public int Id { get; set; } = 1;
    public bool IsDarkTheme { get; set; } = true;
    public string AccentColor { get; set; } = "#2196F3";
    public bool DataMigrated { get; set; }
}
