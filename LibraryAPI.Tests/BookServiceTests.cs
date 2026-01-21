
using Microsoft.EntityFrameworkCore;
using LibraryAPI.Services;
using LibraryAPI.Data;
using LibraryAPI.Models;
using LibraryAPI.Models.DTO;

namespace LibraryAPI.Tests;
public class BookServiceTests
{
    private LibraryDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<LibraryDbContext>()
     .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
     .Options;
        return new LibraryDbContext(options);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllBooks()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        context.Books.AddRange(new List<Book>
        {
            new Book { Title = "Book A", Author = "Author A", ISBN = "ISBN001", PublishedYear = 2000 },
            new Book { Title = "Book B", Author = "Author B", ISBN = "ISBN002", PublishedYear = 2001 }
        });

        await context.SaveChangesAsync();
        var service = new BookService(context);

        // Act
        var books = await service.GetAllAsync(CancellationToken.None);
        // Assert
        Assert.Equal(2, books.Count);
    }

    [Fact]
    public async Task CreateAsync_ValidBook_CreatesSuccessfully()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = new BookService(context);

        var request = new CreateBookRequest
        {
            Title = "Clean Code",
            Author = "Robert Martin",
            ISBN = "9780132350884",
            PublishedYear = 2008
        };

        // Act
        var result = await service.CreateAsync(request, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Clean Code", result.Title);
        Assert.True(result.isAvailable); // Should be available by default
    }

    [Fact]
    public async Task SearchAsync_FindsBookByTitle()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = new BookService(context);

        context.Books.AddRange(
            new Book { Title = "Clean Code", Author = "Robert Martin", ISBN = "1111111111", PublishedYear = 2008 },
            new Book { Title = "Dirty Code", Author = "Someone Else", ISBN = "2222222222", PublishedYear = 2010 }
        );
        await context.SaveChangesAsync();

        // Act
        var result = await service.SearchAsync("Clean", CancellationToken.None);

        // Assert
        Assert.Single(result);
        Assert.Equal("Clean Code", result[0].Title);
    }

    [Fact]

    public async Task GetByIdAsync_ExistingBook_ReturnsBook()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = new BookService(context);
        var book = new Book
        {
            Title = "The Pragmatic Programmer",
            Author = "Andrew Hunt",
            ISBN = "020161622X",
            PublishedYear = 1999
        };
        context.Books.Add(book);
        await context.SaveChangesAsync();
        // Act
        var result = await service.GetByIdAsync(book.Id, CancellationToken.None);
        // Assert
        Assert.NotNull(result);
        Assert.Equal("The Pragmatic Programmer", result.Title);
    }

    [Fact]

    public async Task GetByIdAsync_NonExistingBook_ReturnsNull()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = new BookService(context);
        // Act
        var result = await service.GetByIdAsync(999, CancellationToken.None);
        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_ExistingBook_DeletesSuccessfully()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = new BookService(context);
        var book = new Book
        {
            Title = "Test Driven Development",
            Author = "Kent Beck",
            ISBN = "0321146530",
            PublishedYear = 2002
        };
        context.Books.Add(book);
        await context.SaveChangesAsync();
        // Act
        var result = await service.DeleteAsync(book.Id, CancellationToken.None);
        // Assert
        Assert.True(result);
        var deletedBook = await service.GetByIdAsync(book.Id, CancellationToken.None);
        Assert.Null(deletedBook);
    }

    [Fact]

    public async Task DeleteAsync_NonExistingBook_ReturnsFalse()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = new BookService(context);
        // Act
        var result = await service.DeleteAsync(999, CancellationToken.None);
        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task UpdateAsync_ExistingBook_UpdatesSuccessfully()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = new BookService(context);
        var book = new Book
        {
            Title = "Original Title",
            Author = "Original Author",
            ISBN = "0000000000",
            PublishedYear = 2000
        };
        context.Books.Add(book);
        await context.SaveChangesAsync();
        var updateRequest = new CreateBookRequest
        {
            Title = "Updated Title",
            Author = "Updated Author",
            ISBN = "1111111111",
            PublishedYear = 2020,
        };
        // Act
        var result = await service.UpdateAsync(book.Id, updateRequest, CancellationToken.None);
        // Assert
        Assert.NotNull(result);
        Assert.Equal("Updated Title", result.Title);
        Assert.Equal("Updated Author", result.Author);
        Assert.Equal("1111111111", result.ISBN);
        Assert.Equal(2020, result.PublishedYear);
    }

    [Fact]
    public async Task UpdateAsync_NonExistingBook_ReturnsNull()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = new BookService(context);
        var updateRequest = new CreateBookRequest
        {
            Title = "Non-Existent Book",
            Author = "No Author",
            ISBN = "9999999999",
            PublishedYear = 2021,
        };
        // Act
        var result = await service.UpdateAsync(999, updateRequest, CancellationToken.None);
        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task SearchAsync_NoMatches_ReturnsEmptyList()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = new BookService(context);
        context.Books.AddRange(
            new Book { Title = "Clean Code", Author = "Robert Martin", ISBN = "1111111111", PublishedYear = 2008 },
            new Book { Title = "Dirty Code", Author = "Someone Else", ISBN = "2222222222", PublishedYear = 2010 }
        );
        await context.SaveChangesAsync();
        // Act
        var result = await service.SearchAsync("NonExistentTitle", CancellationToken.None);
        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetAvailableBooksAsync_ReturnsOnlyAvailableBooks()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = new BookService(context);
        context.Books.AddRange(
            new Book { Title = "Available Book", Author = "Author A", ISBN = "3333333333", PublishedYear = 2015, isAvailable = true },
            new Book { Title = "Unavailable Book", Author = "Author B", ISBN = "4444444444", PublishedYear = 2016, isAvailable = false }
        );
        await context.SaveChangesAsync();
        // Act
        var result = await service.GetAvailableBooksAsync(CancellationToken.None);
        // Assert
        Assert.Single(result);
        Assert.Equal("Available Book", result[0].Title);
    }

    [Theory]
    [InlineData("Design Patterns", "Gang of Four", "1234567890", 1994)]
    [InlineData("Refactoring", "Martin Fowler", "0987654321", 1999)]
    public async Task CreateAsync_DifferentBooks_CreatesCorrectly(
        string title, string author, string isbn, int year)
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = new BookService(context);

        var request = new CreateBookRequest
        {
            Title = title,
            Author = author,
            ISBN = isbn,
            PublishedYear = year
        };

        // Act
        var result = await service.CreateAsync(request, CancellationToken.None);

        // Assert
        Assert.Equal(title, result.Title);
        Assert.Equal(author, result.Author);
        Assert.Equal(isbn, result.ISBN);
        Assert.Equal(year, result.PublishedYear);
    }
}

