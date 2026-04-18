using LibraryBookBorrowingSystem.Api.Data;
using LibraryBookBorrowingSystem.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryBookBorrowingSystem.Api.Repositories;

public class BorrowRecordRepository : IBorrowRecordRepository
{
    private readonly ApplicationDbContext _context;

    public BorrowRecordRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<BorrowRecord>> GetAllAsync()
    {
        return await _context.BorrowRecords.AsNoTracking().ToListAsync();
    }

    public async Task<BorrowRecord?> GetByIdAsync(Guid id)
    {
        return await _context.BorrowRecords.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<List<BorrowRecord>> GetByMemberIdAsync(Guid memberId)
    {
        return await _context.BorrowRecords
            .AsNoTracking()
            .Where(r => r.MemberId == memberId)
            .ToListAsync();
    }

    public async Task<BorrowRecord> AddAsync(BorrowRecord record)
    {
        _context.BorrowRecords.Add(record);
        await _context.SaveChangesAsync();
        return record;
    }

    public async Task<BorrowRecord?> UpdateAsync(BorrowRecord record)
    {
        _context.BorrowRecords.Update(record);
        await _context.SaveChangesAsync();
        return record;
    }

    public Task<bool> HasActiveBorrowAsync(Guid memberId, Guid bookId)
    {
        return _context.BorrowRecords.AnyAsync(r =>
            r.MemberId == memberId &&
            r.BookId == bookId &&
            r.Status == "Borrowed");
    }
}