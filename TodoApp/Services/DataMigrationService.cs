using System.IO;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TodoApp.Data;
using TodoApp.Models;

namespace TodoApp.Services;

public class DataMigrationService
{
    // Struktura starého JSON souboru
    private class LegacyTask
    {
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public DateTime Deadline { get; set; }
        public bool Completion { get; set; }
    }

    public async Task MigrateIfNeededAsync()
    {
        using var db = new AppDbContext();
        await db.Database.EnsureCreatedAsync();

        var settings = await db.Settings.FindAsync(1);
        if (settings?.DataMigrated == true)
            return;

        var jsonPath = FindTaskDataJson();
        if (jsonPath == null)
        {
            // Žádný starý soubor nenalezen, jen označíme migraci jako hotovou
            if (settings != null)
            {
                settings.DataMigrated = true;
                await db.SaveChangesAsync();
            }
            return;
        }

        var json = await File.ReadAllTextAsync(jsonPath);
        var legacyTasks = JsonConvert.DeserializeObject<List<LegacyTask>>(json);
        if (legacyTasks == null || legacyTasks.Count == 0)
        {
            if (settings != null)
            {
                settings.DataMigrated = true;
                await db.SaveChangesAsync();
            }
            return;
        }

        var categories = await db.Categories.ToDictionaryAsync(c => c.Name, c => c.Id);

        foreach (var legacy in legacyTasks)
        {
            if (!categories.TryGetValue(legacy.Category, out var categoryId))
            {
                // Vytvořit novou kategorii pokud neexistuje
                var newCat = new Category { Name = legacy.Category, Color = "#9E9E9E" };
                db.Categories.Add(newCat);
                await db.SaveChangesAsync();
                categories[legacy.Category] = newCat.Id;
                categoryId = newCat.Id;
            }

            db.Tasks.Add(new TodoTask
            {
                Title = legacy.Description,
                Deadline = legacy.Deadline,
                IsCompleted = legacy.Completion,
                CompletedAt = legacy.Completion ? DateTime.Now : null,
                CategoryId = categoryId,
                Priority = Priority.Medium,
                CreatedAt = DateTime.Now
            });
        }

        if (settings != null)
            settings.DataMigrated = true;

        await db.SaveChangesAsync();
    }

    private static string? FindTaskDataJson()
    {
        // Hledáme taskdata.json v několika možných umístěních
        var candidates = new[]
        {
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "taskdata.json"),
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "Projekt-TODOList", "bin", "Debug", "net7.0-windows", "taskdata.json"),
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "Projekt-TODOList", "bin", "Release", "net7.0-windows", "taskdata.json"),
        };

        return candidates.FirstOrDefault(File.Exists);
    }
}
