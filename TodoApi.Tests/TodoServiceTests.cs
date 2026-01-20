using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TodoApi.Data;
using TodoApi.Services;
using TodoApi.Models;


namespace TodoApi.Tests
{
    public class TodoServiceTests
    {
        private TodoDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<TodoDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new TodoDbContext(options);
        }

        [Fact]
        public async Task GetAllAsync_ReturnAllTodos()
        {
            //Arrange
            var context = GetInMemoryDbContext();
            var service = new TodoService(context);

            //Add test data
            context.Todos.AddRange(
                new TodoItem { Title = "Test 1", IsCompleted = false },
                new TodoItem { Title = "Test 2", IsCompleted = true }
                );
            await context.SaveChangesAsync();
            //Act
            var result = await service.GetAllAsync();

            //Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task GetByIdAsync_ExistingId_ReturnsTodo()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var service = new TodoService(context);

            var todo = new TodoItem { Title = "Test Task", IsCompleted = false };
            context.Todos.Add(todo);
            await context.SaveChangesAsync();

            // Act
            var result = await service.GetByIdAsync(todo.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Task", result.Title);
        }
        [Fact]
        public async Task GetByIdAsync_NonExistingId_ReturnsNull()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var service = new TodoService(context);

            // Act
            var result = await service.GetByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateAsync_ValidTodo_CreatesAndReturnsTodo()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var service = new TodoService(context);

            var newTodo = new TodoItem
            {
                Title = "New Task",
                IsCompleted = false
            };

            // Act
            var result = await service.CreateAsync(newTodo);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Id > 0); // ID was generated
            Assert.Equal("New Task", result.Title);

            // Verify it's in database
            var savedTodo = await context.Todos.FindAsync(result.Id);
            Assert.NotNull(savedTodo);
        }

        [Fact]
        public async Task UpdateAsync_ExistingTodo_UpdatesSuccessfully()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var service = new TodoService(context);

            var todo = new TodoItem { Title = "Original", IsCompleted = false };
            context.Todos.Add(todo);
            await context.SaveChangesAsync();

            // Act
            var updatedTodo = new TodoItem
            {
                Title = "Updated",
                IsCompleted = true
            };
            var result = await service.UpdateAsync(todo.Id, updatedTodo);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated", result.Title);
            Assert.True(result.IsCompleted);
        }

        [Fact]
        public async Task DeleteAsync_ExistingTodo_DeletesSuccessfully()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var service = new TodoService(context);

            var todo = new TodoItem { Title = "To Delete", IsCompleted = false };
            context.Todos.Add(todo);
            await context.SaveChangesAsync();

            // Act
            var result = await service.DeleteAsync(todo.Id);

            // Assert
            Assert.True(result);

            // Verify it's deleted
            var deletedTodo = await context.Todos.FindAsync(todo.Id);
            Assert.Null(deletedTodo);
        }

        [Fact]
        public async Task DeleteAsync_NonExistingTodo_ReturnsFalse()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var service = new TodoService(context);

            // Act
            var result = await service.DeleteAsync(999);

            // Assert
            Assert.False(result);
        }

        [Theory]
        [InlineData("Buy milk", false)]
        [InlineData("Complete project", true)]
        [InlineData("Call mom", false)]
        public async Task CreateAsync_DifferentTodos_CreatesCorrectly(string title, bool isCompleted)
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var service = new TodoService(context);

            var newTodo = new TodoItem
            {
                Title = title,
                IsCompleted = isCompleted
            };

            // Act
            var result = await service.CreateAsync(newTodo);

            // Assert
            Assert.Equal(title, result.Title);
            Assert.Equal(isCompleted, result.IsCompleted);
        }
    }
}
