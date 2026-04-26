using LibraryBookBorrowingSystem.Api.Dtos;
using LibraryBookBorrowingSystem.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibraryBookBorrowingSystem.Api.Controllers;

[ApiController]
[Route("api")]
public class BorrowRecordController : ControllerBase
{
    private readonly IBorrowRecordService _borrowRecordService;

    public BorrowRecordController(IBorrowRecordService borrowRecordService)
    {
        _borrowRecordService = borrowRecordService;
    }

    [HttpPost("borrow")]
    public async Task<ActionResult<BorrowRecordResponse>> Borrow([FromBody] BorrowRequest request)
    {
        var result = await _borrowRecordService.BorrowBookAsync(request);
        if (result.Record is null)
        {
            var error = new ErrorResponse(result.ErrorMessage ?? "Invalid request.");
            return result.ErrorType switch
            {
                BorrowErrorType.BookNotFound => NotFound(error),
                BorrowErrorType.MemberNotFound => NotFound(error),
                BorrowErrorType.NoAvailableCopies => Conflict(error),
                BorrowErrorType.AlreadyBorrowed => Conflict(error),
                _ => BadRequest(error)
            };
        }

        return Created($"/api/borrow/{result.Record.Id}", result.Record);
    }

    [HttpPost("return/{recordId:guid}")]
    public async Task<ActionResult<BorrowRecordResponse>> Return(Guid recordId, [FromQuery] Guid memberId)
    {
        var result = await _borrowRecordService.ReturnBookAsync(recordId, memberId);
        if (result.Record is null)
        {
            var error = new ErrorResponse(result.ErrorMessage ?? "Invalid request.");
            return result.ErrorType switch
            {
                BorrowErrorType.RecordNotFound => NotFound(error),
                BorrowErrorType.NotBorrowedByMember => BadRequest(error),
                _ => BadRequest(error)
            };
        }

        return Ok(result.Record);
    }

    [HttpGet("borrow-records")]
    public async Task<ActionResult<List<BorrowRecordResponse>>> GetAll()
    {
        var records = await _borrowRecordService.GetAllAsync();
        return Ok(records);
    }

    [HttpGet("members/{memberId:guid}/borrow-history")]
    public async Task<ActionResult<List<BorrowRecordResponse>>> GetByMember(Guid memberId)
    {
        var records = await _borrowRecordService.GetByMemberIdAsync(memberId);
        return Ok(records);
    }
}
