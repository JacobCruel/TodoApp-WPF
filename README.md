# TodoApp — Modern WPF Task Manager

A feature-rich desktop TODO application built with **WPF** and **.NET 8**, following the **MVVM** architectural pattern.

> Student project — **Czech University of Life Sciences Prague** (CZU), Faculty of Economics and Management, **Informatics**

## Features

- **Task Management** — Create, edit, delete tasks with title, notes, deadline, priority levels (Low / Medium / High / Critical), and recurrence (daily / weekly / monthly)
- **Categories & Tags** — Organize tasks with color-coded categories and tags
- **Subtasks** — Break down tasks into smaller checklist items
- **Dashboard** — Overview with statistics: total tasks, completed, overdue, due this week, and a completion progress bar
- **Calendar View** — Monthly grid displaying tasks on their deadline dates; click a day to create a task
- **Search & Filter** — Real-time search, filter by category, sort by deadline / priority / category / completion
- **Dark / Light Theme** — Toggle between themes with customizable accent color
- **Undo / Redo** — Command-pattern based undo/redo for all task operations
- **CSV Export** — Export tasks to CSV format
- **Keyboard Shortcuts** — `Ctrl+N` new task, `Ctrl+Z/Y` undo/redo, `Ctrl+D` toggle theme, `Ctrl+E` export, `Ctrl+1-4` navigation
- **Toast Notifications** — In-app feedback for all actions
- **Data Migration** — Automatically imports data from the legacy WinForms version on first launch

## Tech Stack

| Component | Technology |
|-----------|-----------|
| Framework | .NET 8 (WPF) |
| Architecture | MVVM |
| MVVM Toolkit | CommunityToolkit.Mvvm 8.4.0 |
| Database | SQLite via Entity Framework Core 8 |
| Data location | `%LocalAppData%/TodoApp/todo.db` |

## Build & Run

**Prerequisites:** [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

```bash
# Build
dotnet build TodoApp/TodoApp.csproj

# Run
dotnet run --project TodoApp/TodoApp.csproj

# Publish as single-file executable (no .NET installation required)
dotnet publish TodoApp/TodoApp.csproj -c Release -r win-x64 --self-contained -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o ./publish
```

## Project Structure

```
TodoApp/
├── Models/          — TodoTask, Category, Tag, SubTask, Priority, RecurrenceType
├── Data/            — EF Core DbContext with SQLite
├── Services/        — TaskService, UndoRedoService, ExportService, DataMigrationService
├── ViewModels/      — MainViewModel, TaskListViewModel, DashboardViewModel, CalendarViewModel, SettingsViewModel
├── Views/           — XAML views with data binding
├── Converters/      — Value converters (PriorityToColor, BoolToVisibility, etc.)
└── Resources/
    ├── Themes/      — DarkTheme.xaml, LightTheme.xaml
    ├── Styles/      — Button, TextBox, Card, CheckBox styles
    └── Icons/       — SVG path geometries
```

## Screenshots

*Coming soon*

## License

This project was created for educational purposes at CZU Prague.
