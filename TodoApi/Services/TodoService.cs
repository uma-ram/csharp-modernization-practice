namespace TodoApi.Services;
using TodoApi.Models;

public class TodoService: ITodoService
{
    private readonly List<TodoItem> _todos = new();
    private int _nextId = 1;

    public List<TodoItem> GetAll()
    {
        return _todos;
    }

    public TodoItem? GetById(int id)
    {
        return _todos.FirstOrDefault(t => t.Id == id);
    }

    public TodoItem Create(TodoItem item)
    {
        item.Id = _nextId++;
        item.CreatedAt = DateTime.UtcNow;
        _todos.Add(item);
        return item;
    }

    public TodoItem? Update(int id, TodoItem item)
    {
        var existing = GetById(id);
        if (existing == null) return null;

        existing.Title = item.Title;
        existing.IsCompleted = item.IsCompleted;
        return existing;
    }
    public bool Delete(int id)
    {
        var item = GetById(id);
        if (item == null) return false;

        _todos.Remove(item);
        return true;
    }
}
