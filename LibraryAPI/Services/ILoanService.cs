namespace LibraryAPI.Services;

using LibraryAPI.Models;
using LibraryAPI.Models.DTO;

public interface ILoanService
{
    Task<LoanResponse> LoanBookAsync(LoanBookRequest loanBookRequest, CancellationToken cancellationToken = default);
    Task<LoanResponse> ReturnBookAsync(int loanId, CancellationToken cancellationToken = default);
    Task<List<LoanResponse>> GetActiveLoansAsync(CancellationToken cancellationToken = default);
    Task<List<LoanResponse>> GetMemberLoanHistoryAsync(int memberId, CancellationToken cancellationToken = default);

    Task<List<LoanResponse>> GetOverdueLoansAsync(CancellationToken cancellationToken = default);
}
