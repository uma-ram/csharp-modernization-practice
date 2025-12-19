using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Models;
using TodoApi.Services;
using TodoApi.Extensions;
using TodoApi.Middleware;
using TodoApi.Exceptions;

var builder = WebApplication.CreateBuilder(args);

// Add database
builder.Services.AddDbContext<TodoDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ITodoService, TodoService>();

var app = builder.Build();

// Add global exception handler
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// GET /api/todos - Get all todos
app.MapGet("/api/todos", async (ITodoService service, CancellationToken cancellationToken) =>
{
    var todos = await service.GetAllAsync(cancellationToken);
    return Results.Ok(todos);
})
.WithName("GetAllTodos");

// GET /api/todos/{id} - Get todo by id
app.MapGet("/api/todos/{id}", async (int id, ITodoService service, CancellationToken cancellationToken) =>
{
    var todo = await service.GetByIdAsync(id, cancellationToken);

    if (todo == null)
    {
        throw new TodoNotFoundException(id);
    }

    return Results.Ok(todo);
})
.WithName("GetTodoById");

// POST /api/todos - Create new todo
app.MapPost("/api/todos", async (CreateTodoRequest request, ITodoService service, CancellationToken cancellationToken) =>
{
    var (isValid, errors) = request.Validate();

    if (!isValid)
    {
        throw new ValidationException(errors);
    }

    var item = new TodoItem
    {
        Title = request.Title,
        Description = request.Description,
        IsCompleted = false
    };

    var created = await service.CreateAsync(item, cancellationToken);
    return Results.Created($"/api/todos/{created.Id}", created);
})
.WithName("CreateTodo");

// PUT /api/todos/{id} - Update todo
app.MapPut("/api/todos/{id}", async (int id, TodoItem item, ITodoService service, CancellationToken cancellationToken) =>
{
    var updated = await service.UpdateAsync(id, item, cancellationToken);

    if (updated == null)
    {
        throw new TodoNotFoundException(id);
    }

    return Results.Ok(updated);
})
.WithName("UpdateTodo");

// DELETE /api/todos/{id} - Delete todo
app.MapDelete("/api/todos/{id}", async (int id, ITodoService service, CancellationToken cancellationToken) =>
{
    var deleted = await service.DeleteAsync(id, cancellationToken);

    if (!deleted)
    {
        throw new TodoNotFoundException(id);
    }

    return Results.NoContent();
})
.WithName("DeleteTodo");

// GET /api/todos/completed - Get completed todos
app.MapGet("/api/todos/completed", async (ITodoService service, CancellationToken cancellationToken) =>
{
    var todos = await service.GetCompletedAsync(cancellationToken);
    return Results.Ok(todos);
})
.WithName("GetCompletedTodos");

// GET /api/todos/search?query=... - Search todos
app.MapGet("/api/todos/search", async (string query, ITodoService service, CancellationToken cancellationToken) =>
{
    if (string.IsNullOrWhiteSpace(query))
    {
        return Results.BadRequest(new { message = "Query parameter is required" });
    }

    var todos = await service.SearchAsync(query, cancellationToken);
    return Results.Ok(todos);
})
.WithName("SearchTodos");

// GET /api/todos/stats - Get statistics
app.MapGet("/api/todos/stats", async (ITodoService service, CancellationToken cancellationToken) =>
{
    var allTask = service.GetAllAsync(cancellationToken);
    var countTask = service.GetCountAsync(cancellationToken);
    var completedTask = service.GetCompletedAsync(cancellationToken);

    await Task.WhenAll(allTask, countTask, completedTask);

    var all = await allTask;
    var total = await countTask;
    var completed = await completedTask;

    var stats = new
    {
        total,
        completed = completed.Count,
        pending = total - completed.Count,
        completionRate = total > 0 ? (double)completed.Count / total * 100 : 0
    };

    return Results.Ok(stats);
})
.WithName("GetStats");

app.Run();