using LibraryBookBorrowingSystem.Api.Dtos;

namespace LibraryBookBorrowingSystem.Api.Services;

public interface IMemberService
{
    Task<List<MemberResponse>> GetAllAsync();
    Task<MemberResponse?> GetByIdAsync(Guid id);
    Task<MemberResponse> CreateAsync(CreateMemberRequest request);
    Task<MemberResponse?> UpdateAsync(Guid id, UpdateMemberRequest request);
    Task<bool> DeleteAsync(Guid id);
}
