namespace LibraryBookBorrowingSystem.Api.Dtos;

public class BorrowRequest
{
    public Guid MemberId { get; set; }
    public Guid BookId { get; set; }
}
