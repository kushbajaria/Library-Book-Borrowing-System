using LibraryBookBorrowingSystem.Api.Dtos;

namespace LibraryBookBorrowingSystem.Api.Services;

public interface IBookService
{
    Task<List<BookResponse>> GetAllAsync();
    Task<BookResponse?> GetByIdAsync(Guid id);
    Task<BookResponse> CreateAsync(CreateBookRequest request);
    Task<BookResponse?> UpdateAsync(Guid id, UpdateBookRequest request);
    Task<bool> DeleteAsync(Guid id);
}
