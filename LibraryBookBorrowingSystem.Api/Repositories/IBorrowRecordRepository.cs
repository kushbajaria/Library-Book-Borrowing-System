using LibraryBookBorrowingSystem.Api.Models;

namespace LibraryBookBorrowingSystem.Api.Repositories;

public interface IBorrowRecordRepository
{
    Task<List<BorrowRecord>> GetAllAsync();
    Task<BorrowRecord?> GetByIdAsync(Guid id);
    Task<List<BorrowRecord>> GetByMemberIdAsync(Guid memberId);
    Task<BorrowRecord> AddAsync(BorrowRecord record);
    Task<BorrowRecord?> UpdateAsync(BorrowRecord record);
    Task<bool> HasActiveBorrowAsync(Guid memberId, Guid bookId);
}