using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Service.Book.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookController : ControllerBase
{
    private readonly BookContext _context;

    public BookController(BookContext context)
    {
        _context = context;
    }

    [HttpGet("ListBooks")]
    public async Task<IActionResult> GetAllBooks()
    {
        return Ok(await _context.Books.ToListAsync());
    }

    [HttpPost("AddBook")]
    public async Task<IActionResult> AddBook(CreateBook createBook)
    {
        int nextId = (_context.Books.Any() ? await _context.Books.MaxAsync(b => b.Id) : 0) + 1;

        var book = new Book
        {
            Id = nextId,
            Title = createBook.Title,
            Author = createBook.Author,
            isAvailable = true,
            UserId = null
        };
        _context.Books.Add(book);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAllBooks), new { id = book.Id }, book);
    }
}