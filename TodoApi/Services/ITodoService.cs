using TodoApi.Models;

namespace TodoApi.Services;

public interface ITodoService
{
    Task<List<TodoItem>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TodoItem?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<TodoItem> CreateAsync(TodoItem newTodo, CancellationToken cancellationToken = default);
    Task<TodoItem?> UpdateAsync(int id, TodoItem updatedTodo, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<int> GetCountAsync(CancellationToken cancellationToken = default);
    Task<List<TodoItem>> GetCompletedAsync(CancellationToken cancellationToken = default);
    Task<List<TodoItem>> SearchAsync(string query, CancellationToken cancellationToken = default);

}
