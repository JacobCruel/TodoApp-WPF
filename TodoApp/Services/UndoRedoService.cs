using TodoApp.Data;
using TodoApp.Models;

namespace TodoApp.Services;

public interface IUndoableAction
{
    string Description { get; }
    Task ExecuteAsync();
    Task UndoAsync();
}

public class UndoRedoService
{
    private readonly Stack<IUndoableAction> _undoStack = new();
    private readonly Stack<IUndoableAction> _redoStack = new();
    private const int MaxStackSize = 30;

    public bool CanUndo => _undoStack.Count > 0;
    public bool CanRedo => _redoStack.Count > 0;
    public string? UndoDescription => _undoStack.TryPeek(out var action) ? action.Description : null;
    public string? RedoDescription => _redoStack.TryPeek(out var action) ? action.Description : null;

    public event Action? StateChanged;

    public async Task ExecuteAsync(IUndoableAction action)
    {
        await action.ExecuteAsync();
        _undoStack.Push(action);
        _redoStack.Clear();

        while (_undoStack.Count > MaxStackSize)
        {
            var items = _undoStack.ToArray();
            _undoStack.Clear();
            for (int i = items.Length - 2; i >= 0; i--)
                _undoStack.Push(items[i]);
        }

        StateChanged?.Invoke();
    }

    public async Task UndoAsync()
    {
        if (!CanUndo) return;
        var action = _undoStack.Pop();
        await action.UndoAsync();
        _redoStack.Push(action);
        StateChanged?.Invoke();
    }

    public async Task RedoAsync()
    {
        if (!CanRedo) return;
        var action = _redoStack.Pop();
        await action.ExecuteAsync();
        _undoStack.Push(action);
        StateChanged?.Invoke();
    }
}

public class CreateTaskAction : IUndoableAction
{
    private readonly TaskService _taskService;
    private readonly TodoTask _task;
    private int _createdId;

    public string Description => $"Add '{_task.Title}'";

    public CreateTaskAction(TaskService taskService, TodoTask task)
    {
        _taskService = taskService;
        _task = task;
    }

    public async Task ExecuteAsync()
    {
        var created = await _taskService.CreateTaskAsync(_task);
        _createdId = created.Id;
        _task.Id = _createdId;
    }

    public async Task UndoAsync()
    {
        await _taskService.DeleteTaskAsync(_createdId);
    }
}

public class DeleteTaskAction : IUndoableAction
{
    private readonly TaskService _taskService;
    private readonly TodoTask _snapshot;

    public string Description => $"Delete '{_snapshot.Title}'";

    public DeleteTaskAction(TaskService taskService, TodoTask snapshot)
    {
        _taskService = taskService;
        _snapshot = snapshot;
    }

    public async Task ExecuteAsync()
    {
        await _taskService.DeleteTaskAsync(_snapshot.Id);
    }

    public async Task UndoAsync()
    {
        _snapshot.Id = 0; // Reset ID pro nový insert
        var restored = await _taskService.CreateTaskAsync(_snapshot);
        _snapshot.Id = restored.Id;
    }
}

public class ToggleCompleteAction : IUndoableAction
{
    private readonly TaskService _taskService;
    private readonly int _taskId;
    private readonly string _taskTitle;

    public string Description => $"Toggle '{_taskTitle}'";

    public ToggleCompleteAction(TaskService taskService, int taskId, string taskTitle)
    {
        _taskService = taskService;
        _taskId = taskId;
        _taskTitle = taskTitle;
    }

    public async Task ExecuteAsync()
    {
        await _taskService.ToggleCompleteAsync(_taskId);
    }

    public async Task UndoAsync()
    {
        await _taskService.ToggleCompleteAsync(_taskId);
    }
}
