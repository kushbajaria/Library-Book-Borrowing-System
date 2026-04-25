namespace LibraryBookBorrowingSystem.Api.Dtos;

public class CreateMemberRequest
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
