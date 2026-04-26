using LibraryBookBorrowingSystem.Api.Dtos;
using LibraryBookBorrowingSystem.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibraryBookBorrowingSystem.Api.Controllers;

[ApiController]
[Route("api/members")]
public class MembersController : ControllerBase
{
    private readonly IMemberService _memberService;

    public MembersController(IMemberService memberService)
    {
        _memberService = memberService;
    }

    [HttpGet]
    public async Task<ActionResult<List<MemberResponse>>> GetAll()
    {
        var members = await _memberService.GetAllAsync();
        return Ok(members);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<MemberResponse>> GetById(Guid id)
    {
        var member = await _memberService.GetByIdAsync(id);
        if (member is null) return NotFound();
        return Ok(member);
    }

    [HttpPost]
    public async Task<ActionResult<MemberResponse>> Create([FromBody] CreateMemberRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FullName))
            return BadRequest(new ErrorResponse("FullName is required."));

        if (string.IsNullOrWhiteSpace(request.Email))
            return BadRequest(new ErrorResponse("Email is required."));

        if (!request.Email.Contains("@"))
            return BadRequest(new ErrorResponse("Email is not valid."));

        var created = await _memberService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<MemberResponse>> Update(Guid id, [FromBody] UpdateMemberRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FullName))
            return BadRequest(new ErrorResponse("FullName is required."));

        if (string.IsNullOrWhiteSpace(request.Email) || !request.Email.Contains("@"))
            return BadRequest(new ErrorResponse("A valid Email is required."));

        var updated = await _memberService.UpdateAsync(id, request);
        if (updated is null) return NotFound();
        return Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var deleted = await _memberService.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
