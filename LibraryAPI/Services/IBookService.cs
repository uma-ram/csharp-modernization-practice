namespace LibraryAPI.Services;
using LibraryAPI.Models;
using LibraryAPI.Models.DTO;
public interface IBookService
{
    Task<List<Book>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Book?> GetByIdAsync(int id,  CancellationToken cancellationToken = default);
    Task<Book> CreateAsync(CreateBookRequest createBookRequest, CancellationToken cancellationToken = default);
    Task<Book?> UpdateAsync(int id, CreateBookRequest updateBookRequest, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<List<Book>> SearchAsync(string query, CancellationToken cancellationToken = default);
    Task<List<Book>> GetAvailableBooksAsync(CancellationToken cancellationToken = default);
}
