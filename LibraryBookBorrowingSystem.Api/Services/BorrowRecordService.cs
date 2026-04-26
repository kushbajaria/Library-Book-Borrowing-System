using LibraryBookBorrowingSystem.Api.Dtos;
using LibraryBookBorrowingSystem.Api.Models;
using LibraryBookBorrowingSystem.Api.Repositories;

namespace LibraryBookBorrowingSystem.Api.Services;

public class BorrowRecordService : IBorrowRecordService
{
    private readonly IBookRepository _bookRepository;
    private readonly IMemberRepository _memberRepository;
    private readonly IBorrowRecordRepository _borrowRecordRepository;

    public BorrowRecordService(
        IBookRepository bookRepository,
        IMemberRepository memberRepository,
        IBorrowRecordRepository borrowRecordRepository)
    {
        _bookRepository = bookRepository;
        _memberRepository = memberRepository;
        _borrowRecordRepository = borrowRecordRepository;
    }

    public async Task<BorrowResult> BorrowBookAsync(BorrowRequest request)
    {
        var book = await _bookRepository.GetByIdAsync(request.BookId);
        if (book is null)
            return BorrowResult.Failure(BorrowErrorType.BookNotFound, "Book not found.");

        if (!await _memberRepository.ExistsAsync(request.MemberId))
            return BorrowResult.Failure(BorrowErrorType.MemberNotFound, "Member not found.");

        if (book.AvailableCopies <= 0)
            return BorrowResult.Failure(BorrowErrorType.NoAvailableCopies, "No available copies.");

        if (await _borrowRecordRepository.HasActiveBorrowAsync(request.MemberId, request.BookId))
            return BorrowResult.Failure(BorrowErrorType.AlreadyBorrowed, "Member already has this book borrowed.");

        book.AvailableCopies--;
        await _bookRepository.UpdateAsync(book);

        var record = new BorrowRecord
        {
            Id = Guid.NewGuid(),
            BookId = request.BookId,
            MemberId = request.MemberId,
            BorrowDate = DateTime.UtcNow,
            Status = "Borrowed"
        };

        var created = await _borrowRecordRepository.AddAsync(record);
        return BorrowResult.Created(ToResponse(created));
    }

    public async Task<BorrowResult> ReturnBookAsync(Guid recordId, Guid memberId)
    {
        var record = await _borrowRecordRepository.GetByIdAsync(recordId);
        if (record is null)
            return BorrowResult.Failure(BorrowErrorType.RecordNotFound, "Borrow record not found.");

        if (record.MemberId != memberId)
            return BorrowResult.Failure(BorrowErrorType.NotBorrowedByMember, "This record does not belong to this member.");

        if (record.Status == "Returned")
            return BorrowResult.Failure(BorrowErrorType.RecordNotFound, "Book already returned.");

        record.Status = "Returned";
        record.ReturnDate = DateTime.UtcNow;
        await _borrowRecordRepository.UpdateAsync(record);

        var book = await _bookRepository.GetByIdAsync(record.BookId);
        if (book is not null)
        {
            book.AvailableCopies++;
            await _bookRepository.UpdateAsync(book);
        }

        return BorrowResult.Created(ToResponse(record));
    }

    public async Task<List<BorrowRecordResponse>> GetAllAsync()
    {
        var records = await _borrowRecordRepository.GetAllAsync();
        return records.Select(r => ToResponse(r)).ToList();
    }

    public async Task<List<BorrowRecordResponse>> GetByMemberIdAsync(Guid memberId)
    {
        var records = await _borrowRecordRepository.GetByMemberIdAsync(memberId);
        return records.Select(r => ToResponse(r)).ToList();
    }

    private static BorrowRecordResponse ToResponse(BorrowRecord r) => new BorrowRecordResponse
    {
        Id = r.Id,
        BookId = r.BookId,
        MemberId = r.MemberId,
        BorrowDate = r.BorrowDate,
        ReturnDate = r.ReturnDate,
        Status = r.Status
    };
}
