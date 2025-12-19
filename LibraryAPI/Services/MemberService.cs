using Microsoft.EntityFrameworkCore;
using LibraryAPI.Data;
using LibraryAPI.Models;
using LibraryAPI.Models.DTO;
using LibraryAPI.Exceptions;

namespace LibraryAPI.Services;

public class MemberService : IMemberService
{
    private readonly LibraryDbContext _context;
    public MemberService(LibraryDbContext context)
    {
        _context = context;
    }
    public async Task<Member> CreateAsync(CreateMemberRequest createMemberRequest, CancellationToken cancellationToken = default)
    {
        var member = new Member
        {
            Name = createMemberRequest.Name,
            Email = createMemberRequest.Email,
            JoinedDate = DateTime.UtcNow
        };
        _context.Members.Add(member);
        await _context.SaveChangesAsync(cancellationToken);
        return member;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var member = await GetByIdAsync(id, cancellationToken);
        if (member == null) return false;

        _context.Members.Remove(member);

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<List<Member>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Members
            .OrderBy(m => m.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Member?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Members.FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }

    public async  Task<Member?> UpdateAsync(int id, CreateMemberRequest updateMemberRequest, CancellationToken cancellationToken = default)
    {
        var member = await GetByIdAsync(id, cancellationToken);
        if (member == null) return null;

        member.Name = updateMemberRequest.Name;
        member.Email = updateMemberRequest.Email;

        await _context.SaveChangesAsync(cancellationToken);
        return member;

    }
}
