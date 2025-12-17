namespace LibraryAPI.Services;

using LibraryAPI.Models;
using LibraryAPI.Models.DTO;

public interface IMemberService
{
    Task<List<Member>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Member?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Member> CreateAsync(CreateMemberRequest createMemberRequest, CancellationToken cancellationToken = default);
    Task<Member?> UpdateAsync(int id, CreateMemberRequest updateMemberRequest, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
