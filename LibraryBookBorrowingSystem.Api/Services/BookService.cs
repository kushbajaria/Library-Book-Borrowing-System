using LibraryBookBorrowingSystem.Api.Dtos;
using LibraryBookBorrowingSystem.Api.Models;
using LibraryBookBorrowingSystem.Api.Repositories;
using Microsoft.Extensions.Caching.Memory;

namespace LibraryBookBorrowingSystem.Api.Services;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;
    private readonly IMemoryCache _cache;

    // This is the key we use to store/retrieve the books list from cache
    private const string AllBooksCacheKey = "all_books";

    public BookService(IBookRepository bookRepository, IMemoryCache cache)
    {
        _bookRepository = bookRepository;
        _cache = cache;
    }

    public async Task<List<BookResponse>> GetAllAsync()
    {
        // Try to get from cache first
        if (_cache.TryGetValue(AllBooksCacheKey, out List<BookResponse>? cached) && cached is not null)
        {
            return cached;
        }

        // Cache miss - go to database
        var books = await _bookRepository.GetAllAsync();
        var result = books.Select(b => ToResponse(b)).ToList();

        // Store in cache for 2 minutes
        _cache.Set(AllBooksCacheKey, result, TimeSpan.FromMinutes(2));

        return result;
    }

    public async Task<BookResponse?> GetByIdAsync(Guid id)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        if (book is null) return null;
        return ToResponse(book);
    }

    public async Task<BookResponse> CreateAsync(CreateBookRequest request)
    {
        var book = new Book
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Author = request.Author,
            ISBN = request.ISBN,
            TotalCopies = request.TotalCopies,
            AvailableCopies = request.TotalCopies
        };

        var created = await _bookRepository.AddAsync(book);

        // Invalidate cache so next GetAll fetches fresh data
        _cache.Remove(AllBooksCacheKey);

        return ToResponse(created);
    }

    public async Task<BookResponse?> UpdateAsync(Guid id, UpdateBookRequest request)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        if (book is null) return null;

        book.Title = request.Title;
        book.Author = request.Author;
        book.ISBN = request.ISBN;
        book.TotalCopies = request.TotalCopies;

        var updated = await _bookRepository.UpdateAsync(book);

        // Invalidate cache since data changed
        _cache.Remove(AllBooksCacheKey);

        return updated is null ? null : ToResponse(updated);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var deleted = await _bookRepository.DeleteAsync(id);

        // Invalidate cache since data changed
        if (deleted) _cache.Remove(AllBooksCacheKey);

        return deleted;
    }

    private static BookResponse ToResponse(Book b) => new BookResponse
    {
        Id = b.Id,
        Title = b.Title,
        Author = b.Author,
        ISBN = b.ISBN,
        TotalCopies = b.TotalCopies,
        AvailableCopies = b.AvailableCopies
    };
}