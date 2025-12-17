namespace TodoApi.Services;

using TodoApi.Models;
using Microsoft.EntityFrameworkCore;

public class TodoService: ITodoService
{
    private readonly TodoDBContext _context;
    public TodoService(TodoDbContext context)
    {
        _context = context;
    }

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

    public Task<List<TodoItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<TodoItem?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<TodoItem> CreateAsync(TodoItem newTodo, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<TodoItem> UpdateAsync(int id, TodoItem updatedTodo, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<int> GetCountAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<List<TodoItem>> GetCompletedAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<List<TodoItem>> SearchAsync(string query, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
