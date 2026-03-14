using System.IO;
using Microsoft.EntityFrameworkCore;
using TodoApp.Models;

namespace TodoApp.Data;

public class AppDbContext : DbContext
{
    private readonly string _dbPath;

    public DbSet<TodoTask> Tasks => Set<TodoTask>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<TaskTag> TaskTags => Set<TaskTag>();
    public DbSet<SubTask> SubTasks => Set<SubTask>();
    public DbSet<AppSettings> Settings => Set<AppSettings>();

    public AppDbContext()
    {
        var folder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "TodoApp");
        Directory.CreateDirectory(folder);
        _dbPath = Path.Combine(folder, "todo.db");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite($"Data Source={_dbPath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaskTag>()
            .HasKey(tt => new { tt.TodoTaskId, tt.TagId });

        modelBuilder.Entity<TaskTag>()
            .HasOne(tt => tt.TodoTask)
            .WithMany(t => t.TaskTags)
            .HasForeignKey(tt => tt.TodoTaskId);

        modelBuilder.Entity<TaskTag>()
            .HasOne(tt => tt.Tag)
            .WithMany(t => t.TaskTags)
            .HasForeignKey(tt => tt.TagId);

        modelBuilder.Entity<SubTask>()
            .HasOne(s => s.TodoTask)
            .WithMany(t => t.SubTasks)
            .HasForeignKey(s => s.TodoTaskId);

        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "School", Color = "#4CAF50" },
            new Category { Id = 2, Name = "Shopping", Color = "#FF9800" },
            new Category { Id = 3, Name = "Work", Color = "#2196F3" },
            new Category { Id = 4, Name = "Other", Color = "#9E9E9E" }
        );

        modelBuilder.Entity<AppSettings>().HasData(
            new AppSettings { Id = 1, IsDarkTheme = true, AccentColor = "#2196F3" }
        );
    }
}
