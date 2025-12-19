namespace TodoApi.Services;

using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Models;

public class TodoService : ITodoService
{
    private readonly TodoDbContext _context;

    public TodoService(TodoDbContext context)
    {
        _context = context;
    }

    public async Task<List<TodoItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Todos
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<TodoItem?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Todos
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<TodoItem> CreateAsync(TodoItem item, CancellationToken cancellationToken = default)
    {
        item.CreatedAt = DateTime.UtcNow;
        _context.Todos.Add(item);
        await _context.SaveChangesAsync(cancellationToken);
        return item;
    }

    public async Task<TodoItem?> UpdateAsync(int id, TodoItem item, CancellationToken cancellationToken = default)
    {
        var existing = await GetByIdAsync(id, cancellationToken);
        if (existing == null) return null;

        existing.Title = item.Title;
        existing.Description = item.Description;
        existing.IsCompleted = item.IsCompleted;

        if (item.IsCompleted && existing.CompletedAt == null)
        {
            existing.CompletedAt = DateTime.UtcNow;
        }
        else if (!item.IsCompleted)
        {
            existing.CompletedAt = null;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return existing;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var item = await GetByIdAsync(id, cancellationToken);
        if (item == null) return false;

        _context.Todos.Remove(item);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<int> GetCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Todos.CountAsync(cancellationToken);
    }

    public async Task<List<TodoItem>> GetCompletedAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Todos
            .Where(t => t.IsCompleted)
            .OrderByDescending(t => t.CompletedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<TodoItem>> SearchAsync(string query, CancellationToken cancellationToken = default)
    {
        return await _context.Todos
            .Where(t => t.Title.Contains(query) ||
                       (t.Description != null && t.Description.Contains(query)))
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}
