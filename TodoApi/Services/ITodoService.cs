using TodoApi.Models;

namespace TodoApi.Services;

public interface ITodoService
{
    List<TodoItem> GetAll();
    TodoItem? GetById(int id);
    TodoItem Create(TodoItem newTodo);
    TodoItem Update(int id, TodoItem updatedTodo);

    bool Delete(int id);

}
