using TodoApi.Services;
using TodoApi.Models;
using TodoApi.Extensions;   

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<ITodoService, TodoService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//Define Endpoints
app.MapGet("/api/todos", (ITodoService service) =>
{
    return Results.Ok(service.GetAll());    
})
.WithName("GetAllTodos");

//GET /api/todos/{id} - Get todo bt id
app.MapGet("/api/todos/{id}", (int id, ITodoService service) =>
{
    var todo = service.GetById(id);
    return todo is not null ? Results.Ok(todo) : Results.NotFound();
})
.WithName("GetTodoById");


//POST /api/todos - Create new todo
app.MapPost("/api/todos", (CreateTodoRequest newTodo, ITodoService service) =>
{
    var(isValid, errors) = newTodo.Validate();
    if (!isValid)
    {
        return Results.BadRequest(new { Errors = errors });
    }
    var item = new TodoItem
    {
        Title = newTodo.Title,
        IsCompleted = false
    };
    var createdTodo = service.Create(item);
    return Results.Created($"/api/todos/{createdTodo.Id}", createdTodo);
})
.WithName("CreateTodo");

//PUT /api/todos/{id} - Update existing todo
app.MapPut("/api/todos/{id}", (int id, TodoItem updatedTodo, ITodoService service) =>
{
    var todo = service.Update(id, updatedTodo);
    return todo is not null ? Results.Ok(todo) : Results.NotFound();
})
.WithName("UpdateTodo");


//DELETE /api/todos/{id} - Delete todo by id
app.MapDelete("/api/todos/{id}", (int id, ITodoService service) =>
{
    var deleted = service.Delete(id);
    return deleted ? Results.NoContent() : Results.NotFound();
})
.WithName("DeleteTodo");

app.Run();
