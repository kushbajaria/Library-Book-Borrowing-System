using LibraryBookBorrowingSystem.Api.Dtos;

namespace LibraryBookBorrowingSystem.Api.Services;

public enum BorrowErrorType
{
    None = 0,
    BookNotFound = 1,
    MemberNotFound = 2,
    NoAvailableCopies = 3,
    AlreadyBorrowed = 4,
    RecordNotFound = 5,
    NotBorrowedByMember = 6
}

public sealed class BorrowResult
{
    private BorrowResult(BorrowRecordResponse? record, BorrowErrorType errorType, string? errorMessage)
    {
        Record = record;
        ErrorType = errorType;
        ErrorMessage = errorMessage;
    }

    public BorrowRecordResponse? Record { get; }
    public BorrowErrorType ErrorType { get; }
    public string? ErrorMessage { get; }

    public static BorrowResult Created(BorrowRecordResponse record)
        => new BorrowResult(record, BorrowErrorType.None, null);

    public static BorrowResult Failure(BorrowErrorType errorType, string errorMessage)
        => new BorrowResult(null, errorType, errorMessage);
}
