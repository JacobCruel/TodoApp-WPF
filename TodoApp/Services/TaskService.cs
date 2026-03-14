using Microsoft.EntityFrameworkCore;
using TodoApp.Data;
using TodoApp.Models;

namespace TodoApp.Services;

public class TaskService
{
    public async Task<List<TodoTask>> GetAllTasksAsync()
    {
        using var db = new AppDbContext();
        return await db.Tasks
            .Include(t => t.Category)
            .Include(t => t.SubTasks.OrderBy(s => s.Order))
            .Include(t => t.TaskTags)
                .ThenInclude(tt => tt.Tag)
            .OrderBy(t => t.Deadline)
            .ToListAsync();
    }

    public async Task<TodoTask> CreateTaskAsync(TodoTask task)
    {
        using var db = new AppDbContext();
        db.Tasks.Add(task);
        await db.SaveChangesAsync();

        // Načteme zpět s navigačními vlastnostmi
        return (await db.Tasks
            .Include(t => t.Category)
            .Include(t => t.SubTasks)
            .Include(t => t.TaskTags).ThenInclude(tt => tt.Tag)
            .FirstOrDefaultAsync(t => t.Id == task.Id))!;
    }

    public async Task UpdateTaskAsync(TodoTask task)
    {
        using var db = new AppDbContext();
        db.Tasks.Update(task);
        await db.SaveChangesAsync();
    }

    public async Task DeleteTaskAsync(int taskId)
    {
        using var db = new AppDbContext();
        var task = await db.Tasks
            .Include(t => t.SubTasks)
            .Include(t => t.TaskTags)
            .FirstOrDefaultAsync(t => t.Id == taskId);
        if (task != null)
        {
            db.Tasks.Remove(task);
            await db.SaveChangesAsync();
        }
    }

    public async Task<TodoTask?> ToggleCompleteAsync(int taskId)
    {
        using var db = new AppDbContext();
        var task = await db.Tasks.FindAsync(taskId);
        if (task == null) return null;

        task.IsCompleted = !task.IsCompleted;
        task.CompletedAt = task.IsCompleted ? DateTime.Now : null;
        await db.SaveChangesAsync();

        return await db.Tasks
            .Include(t => t.Category)
            .Include(t => t.SubTasks)
            .Include(t => t.TaskTags).ThenInclude(tt => tt.Tag)
            .FirstOrDefaultAsync(t => t.Id == taskId);
    }

    public async Task<List<Category>> GetCategoriesAsync()
    {
        using var db = new AppDbContext();
        return await db.Categories.OrderBy(c => c.Name).ToListAsync();
    }

    public async Task<Category> CreateCategoryAsync(string name, string color)
    {
        using var db = new AppDbContext();
        var category = new Category { Name = name, Color = color };
        db.Categories.Add(category);
        await db.SaveChangesAsync();
        return category;
    }

    public async Task DeleteCategoryAsync(int categoryId)
    {
        using var db = new AppDbContext();
        var category = await db.Categories.FindAsync(categoryId);
        if (category != null)
        {
            db.Categories.Remove(category);
            await db.SaveChangesAsync();
        }
    }

    public async Task<List<Tag>> GetTagsAsync()
    {
        using var db = new AppDbContext();
        return await db.Tags.OrderBy(t => t.Name).ToListAsync();
    }

    public async Task<Tag> CreateTagAsync(string name, string color)
    {
        using var db = new AppDbContext();
        var tag = new Tag { Name = name, Color = color };
        db.Tags.Add(tag);
        await db.SaveChangesAsync();
        return tag;
    }

    public async Task<AppSettings> GetSettingsAsync()
    {
        using var db = new AppDbContext();
        return (await db.Settings.FindAsync(1))!;
    }

    public async Task SaveSettingsAsync(AppSettings settings)
    {
        using var db = new AppDbContext();
        db.Settings.Update(settings);
        await db.SaveChangesAsync();
    }
}
