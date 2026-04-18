using LibraryBookBorrowingSystem.Api.Data;
using LibraryBookBorrowingSystem.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryBookBorrowingSystem.Api.Repositories;

public class MemberRepository : IMemberRepository
{
    private readonly ApplicationDbContext _context;

    public MemberRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Member>> GetAllAsync()
    {
        return await _context.Members.AsNoTracking().ToListAsync();
    }

    public async Task<Member?> GetByIdAsync(Guid id)
    {
        return await _context.Members.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<Member> AddAsync(Member member)
    {
        _context.Members.Add(member);
        await _context.SaveChangesAsync();
        return member;
    }

    public async Task<Member?> UpdateAsync(Member member)
    {
        _context.Members.Update(member);
        await _context.SaveChangesAsync();
        return member;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var member = await _context.Members.FindAsync(id);
        if (member is null) return false;
        _context.Members.Remove(member);
        await _context.SaveChangesAsync();
        return true;
    }

    public Task<bool> ExistsAsync(Guid id)
    {
        return _context.Members.AnyAsync(m => m.Id == id);
    }
}