using LibraryBookBorrowingSystem.Api.Dtos;

namespace LibraryBookBorrowingSystem.Api.Services;

public interface IBorrowRecordService
{
    Task<BorrowResult> BorrowBookAsync(BorrowRequest request);
    Task<BorrowResult> ReturnBookAsync(Guid recordId, Guid memberId);
    Task<List<BorrowRecordResponse>> GetAllAsync();
    Task<List<BorrowRecordResponse>> GetByMemberIdAsync(Guid memberId);
}
