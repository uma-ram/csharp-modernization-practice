namespace LibraryAPI.Services;

using LibraryAPI.Models;
using LibraryAPI.Models.DTO;

public interface ILoanService
{
    Task<Loan> LoanBookAsync(LoanBookRequest loanBookRequest, CancellationToken cancellationToken = default);
    Task<Loan> ReturnBookAsync(int loanId, CancellationToken cancellationToken = default);
    Task<List<Loan>> GetActiveLoansAsync(CancellationToken cancellationToken = default);
    Task<List<Loan>> GetMemberLoanHistoryAsync(int memberId, CancellationToken cancellationToken = default);

    Task<List<Loan>> GetOverdueLoansAsync(CancellationToken cancellationToken = default);
}
