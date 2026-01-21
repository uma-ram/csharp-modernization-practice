using LibraryAPI.Data;
using LibraryAPI.Exceptions;
using LibraryAPI.Models;
using LibraryAPI.Models.DTO;
using LibraryAPI.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryAPI.Tests;

public class LoanServiceTests
{
    private LibraryDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<LibraryDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new LibraryDbContext(options);
    }

    [Fact]
    public async Task LoanBookAsync_ValidRequest_CreatesLoan()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = new LoanService(context);

        var book = new Book
        {
            Title = "Test Book",
            Author = "Test Author",
            ISBN = "1234567890",
            PublishedYear = 2020,
            isAvailable = true
        };
        var member = new Member
        {
            Name = "John Doe",
            Email = "john@example.com"
        };

        context.Books.Add(book);
        context.Members.Add(member);
        await context.SaveChangesAsync();

        var request = new LoanBookRequest
        {
            BookId = book.Id,
            MemberId = member.Id,
            DurationInDays = 14
        };

        // Act
        var result = await service.LoanBookAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(book.Id, result.BookId);
        Assert.Equal(member.Id, result.MemberId);

        // Verify book is no longer available
        var updatedBook = await context.Books.FindAsync(book.Id);
        Assert.False(updatedBook.isAvailable);
    }

    [Fact]
    public async Task LoanBookAsync_BookNotAvailable_ThrowsException()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = new LoanService(context);

        var book = new Book
        {
            Title = "Test Book",
            Author = "Test Author",
            ISBN = "1234567890",
            PublishedYear = 2020,
            isAvailable = false  // Already loaned!
        };
        var member = new Member
        {
            Name = "John Doe",
            Email = "john@example.com"
        };

        context.Books.Add(book);
        context.Members.Add(member);
        await context.SaveChangesAsync();

        var request = new LoanBookRequest
        {
            BookId = book.Id,
            MemberId = member.Id,
            DurationInDays = 14
        };

        // Act & Assert
        await Assert.ThrowsAsync<BookNotAvailableException>(
            () => service.LoanBookAsync(request)
        );
    }

    [Fact]
    public async Task LoanBookAsync_BookNotFound_ThrowsException()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = new LoanService(context);

        var member = new Member
        {
            Name = "John Doe",
            Email = "john@example.com"
        };
        context.Members.Add(member);
        await context.SaveChangesAsync();

        var request = new LoanBookRequest
        {
            BookId = 999,  // Non-existent book
            MemberId = member.Id,
            DurationInDays = 14
        };

        // Act & Assert
        await Assert.ThrowsAsync<BookNotFoundException>(
            () => service.LoanBookAsync(request)
        );
    }

    [Fact]
    public async Task ReturnBookAsync_ValidLoan_MarksAsReturned()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = new LoanService(context);

        var book = new Book
        {
            Title = "Test Book",
            Author = "Test Author",
            ISBN = "1234567890",
            PublishedYear = 2020,
            isAvailable = false
        };
        var member = new Member
        {
            Name = "John Doe",
            Email = "john@example.com"
        };
        var loan = new Loan
        {
            BookId = book.Id,
            Book = book,
            MemberId = member.Id,
            Member = member,
            LoanDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(14),
            ReturnDate = null
        };

        context.Books.Add(book);
        context.Members.Add(member);
        context.Loans.Add(loan);
        await context.SaveChangesAsync();

        // Act
        var result = await service.ReturnBookAsync(loan.Id);

        // Assert
        Assert.NotNull(result.ReturnDate);

        // Verify book is available again
        var updatedBook = await context.Books.FindAsync(book.Id);
        Assert.True(updatedBook.isAvailable);
    }

    [Fact]
    public async Task GetOverdueLoansAsync_ReturnsOnlyOverdueLoans()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var service = new LoanService(context);

        var book1 = new Book { Title = "Book 1", Author = "Author 1", ISBN = "1111111111", PublishedYear = 2020 };
        var book2 = new Book { Title = "Book 2", Author = "Author 2", ISBN = "2222222222", PublishedYear = 2021 };
        var member = new Member { Name = "John Doe", Email = "john@example.com" };

        context.Books.AddRange(book1, book2);
        context.Members.Add(member);
        await context.SaveChangesAsync();

        // Loan 1: Overdue
        var overdueLoan = new Loan
        {
            BookId = book1.Id,
            Book = book1,
            MemberId = member.Id,
            Member = member,
            LoanDate = DateTime.UtcNow.AddDays(-20),
            DueDate = DateTime.UtcNow.AddDays(-5),  // 5 days overdue!
            ReturnDate = null
        };

        // Loan 2: Not overdue
        var activeLoan = new Loan
        {
            BookId = book2.Id,
            Book = book2,
            MemberId = member.Id,
            Member = member,
            LoanDate = DateTime.UtcNow.AddDays(-5),
            DueDate = DateTime.UtcNow.AddDays(10),  // Still have 10 days
            ReturnDate = null
        };

        context.Loans.AddRange(overdueLoan, activeLoan);
        await context.SaveChangesAsync();

        // Act
        var result = await service.GetOverdueLoansAsync();

        // Assert
        Assert.Single(result);  // Only one overdue loan
        Assert.Equal(book1.Id, result[0].BookId);
    }

}
