using LibraryBookBorrowingSystem.Api.Dtos;
using LibraryBookBorrowingSystem.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibraryBookBorrowingSystem.Api.Controllers;

[ApiController]
[Route("api/books")]
public class BooksController : ControllerBase
{
    private readonly IBookService _bookService;

    public BooksController(IBookService bookService)
    {
        _bookService = bookService;
    }

    [HttpGet]
    public async Task<ActionResult<List<BookResponse>>> GetAll()
    {
        var books = await _bookService.GetAllAsync();
        return Ok(books);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BookResponse>> GetById(Guid id)
    {
        var book = await _bookService.GetByIdAsync(id);
        if (book is null) return NotFound();
        return Ok(book);
    }

    [HttpPost]
    public async Task<ActionResult<BookResponse>> Create([FromBody] CreateBookRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
            return BadRequest(new ErrorResponse("Title is required."));

        if (string.IsNullOrWhiteSpace(request.Author))
            return BadRequest(new ErrorResponse("Author is required."));

        if (string.IsNullOrWhiteSpace(request.ISBN))
            return BadRequest(new ErrorResponse("ISBN is required."));

        if (request.TotalCopies <= 0)
            return BadRequest(new ErrorResponse("TotalCopies must be greater than 0."));

        var created = await _bookService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<BookResponse>> Update(Guid id, [FromBody] UpdateBookRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
            return BadRequest(new ErrorResponse("Title is required."));

        if (request.TotalCopies <= 0)
            return BadRequest(new ErrorResponse("TotalCopies must be greater than 0."));

        var updated = await _bookService.UpdateAsync(id, request);
        if (updated is null) return NotFound();
        return Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var deleted = await _bookService.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
