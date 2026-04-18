using LibraryBookBorrowingSystem.Api.Models;

namespace LibraryBookBorrowingSystem.Api.Repositories;

public interface IMemberRepository
{
    Task<List<Member>> GetAllAsync();
    Task<Member?> GetByIdAsync(Guid id);
    Task<Member> AddAsync(Member member);
    Task<Member?> UpdateAsync(Member member);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}