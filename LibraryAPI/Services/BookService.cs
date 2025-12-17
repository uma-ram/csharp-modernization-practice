using LibraryAPI.Data;
using LibraryAPI.Models;
using LibraryAPI.Models.DTO;
using Microsoft.EntityFrameworkCore;
using LibraryAPI.Exceptions;

namespace LibraryAPI.Services
{
    public class BookService : IBookService
    {
        private readonly LibraryDbContext _context;
        public BookService(LibraryDbContext context)
        {
            _context = context;
        }
        public async Task<List<Book>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _context.Books
                .OrderBy(b => b.Title)
                .ToListAsync(cancellationToken);
        }

        public async Task<Book?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.Books.FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
        }

        public async Task<Book> CreateAsync(CreateBookRequest createBookRequest, CancellationToken cancellationToken)
        {
            var book = new Book
            {
                Title = createBookRequest.Title,
                Author = createBookRequest.Author,
                ISBN = createBookRequest.ISBN,
                PublishedYear = createBookRequest.PublishedYear,
                CreatedAt = DateTime.UtcNow,
                isAvailable = true
            };
            _context.Books.Add(book);
            await _context.SaveChangesAsync(cancellationToken);
            return book;

        }

        public async Task<Book?> UpdateAsync(int id, CreateBookRequest updateBookRequest, CancellationToken cancellationToken)
        {
            var book =await GetByIdAsync(id, cancellationToken);
            if (book == null) return null;
            book.Title = updateBookRequest.Title;
            book.Author = updateBookRequest.Author;
            book.ISBN = updateBookRequest.ISBN;
            book.PublishedYear = updateBookRequest.PublishedYear;

            await _context.SaveChangesAsync(cancellationToken);
            return book;
        }
        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            var book = await GetByIdAsync(id, cancellationToken);
            if (book == null) return false;

            _context.Books.Remove(book);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<List<Book>> SearchAsync(string query, CancellationToken cancellationToken)
        {
            return await _context.Books
                .Where(b=>b.Title.Contains(query) || b.Author.Contains(query))
                .OrderBy(b=>b.Title)
                .ToListAsync(cancellationToken);
        }

        public Task<List<Book>> GetAvailableBooksAsync(CancellationToken cancellationToken)
        {
            return _context.Books
                .Where(b => b.isAvailable)
                .OrderBy(b => b.Title)
                .ToListAsync(cancellationToken);
        }


       

       
    }
}
