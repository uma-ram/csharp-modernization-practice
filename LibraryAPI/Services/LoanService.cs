using Microsoft.EntityFrameworkCore;
using LibraryAPI.Data;
using LibraryAPI.Models;
using LibraryAPI.Models.DTO;
using LibraryAPI.Exceptions;
using System.Runtime.InteropServices;

namespace LibraryAPI.Services;

public class LoanService : ILoanService
{
    private readonly LibraryDbContext _context;
    public LoanService(LibraryDbContext context)
    {
        _context = context;
    }

    public async Task<Loan> LoanBookAsync(LoanBookRequest loanBookRequest, CancellationToken cancellationToken = default)
    {
        var book = await _context.Books.FindAsync(new object[] { loanBookRequest.BookId }, cancellationToken);
        if (book == null)
        {
            throw new BookNotFoundException(loanBookRequest.BookId);
        }
        if (!book.isAvailable)
        {
            throw new BookNotAvailableException(loanBookRequest.BookId);
        }
        var member = await _context.Members.FindAsync(new object[] { loanBookRequest.MemberId }, cancellationToken);
        if (member == null)
        {
            throw new MemberNotFoundException(loanBookRequest.MemberId);
        }
        var loan = new Loan
        {
            BookId = loanBookRequest.BookId,
            MemberId = loanBookRequest.MemberId,
            LoanDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(loanBookRequest.DurationInDays)
            Book = book,
            Member = member
        };
        book.isAvailable = false;

        _context.Loans.Add(loan);
        await _context.SaveChangesAsync(cancellationToken);
        // Load navigation properties for response -  this is explicit loading
        //await _context.Entry(loan).Reference(l => l.Book).LoadAsync(cancellationToken);
        //await _context.Entry(loan).Reference(l => l.Member).LoadAsync(cancellationToken);

        return loan;
    }
    public async Task<Loan> ReturnBookAsync(int loanId, CancellationToken cancellationToken = default)
    {
        var loan = await _context.Loans
            .Include(l => l.Book)
            .FirstOrDefaultAsync(l => l.Id == loanId, cancellationToken);
        if (loan == null)
        {
            throw new Exception($"Loan with ID {loanId} not found.");
        }
        if (loan.ReturnDate != null)
        {
            throw new Exception($"Loan with ID {loanId} has already been returned.");
        }
        loan.ReturnDate = DateTime.UtcNow;
        loan.Book.isAvailable = true;
        await _context.SaveChangesAsync(cancellationToken);
        return loan;

    }
    public async Task<List<Loan>> GetActiveLoansAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Loans
            .Include(l => l.Book)
            .Include(l => l.Member)
            .Where(l => l.ReturnDate == null)
            .OrderBy(l => l.DueDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Loan>> GetMemberLoanHistoryAsync(int memberId, CancellationToken cancellationToken = default)
    {
        return await _context.Loans
            .Include(l => l.Book)
            .Where(l => l.MemberId == memberId)
            .OrderByDescending(l => l.LoanDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Loan>> GetOverdueLoansAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await _context.Loans
            .Include(l => l.Book)
            .Include(l => l.Member)
            .Where(l => l.ReturnDate == null && l.DueDate < now)
            .OrderBy(l => l.DueDate)
            .ToListAsync(cancellationToken);
    }
}
