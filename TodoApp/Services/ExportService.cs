using System.IO;
using System.Text;
using TodoApp.Models;

namespace TodoApp.Services;

public class ExportService
{
    public async Task ExportToCsvAsync(IEnumerable<TodoTask> tasks, string filePath)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Title,Category,Priority,Deadline,Completed,Notes");

        foreach (var task in tasks)
        {
            var title = EscapeCsv(task.Title);
            var category = task.Category?.Name ?? "";
            var priority = task.Priority.ToString();
            var deadline = task.Deadline?.ToString("yyyy-MM-dd") ?? "";
            var completed = task.IsCompleted ? "Yes" : "No";
            var notes = EscapeCsv(task.Notes ?? "");

            sb.AppendLine($"{title},{category},{priority},{deadline},{completed},{notes}");
        }

        await File.WriteAllTextAsync(filePath, sb.ToString(), Encoding.UTF8);
    }

    private static string EscapeCsv(string value)
    {
        if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
            return $"\"{value.Replace("\"", "\"\"")}\"";
        return value;
    }
}
