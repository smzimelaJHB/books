using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Book.Models;

[ApiController]
[Route("api")]
public class bookController : ControllerBase
{

    private readonly BookContext _context;
    private Book LastRecordId;

    public bookController(BookContext context)
    {
        _context = context;
    }

    [HttpGet]
    [Route("books")]
    public async Task<ActionResult<IEnumerable<Book>>> Getbook()
        {
            var books = await _context.books.ToListAsync();
            return Ok(books);
        }

    [HttpGet("books/{id}")]
    public async Task<ActionResult<Book>> Getbook(int id)
    {
        var book = await _context.books.FindAsync(id);

        if (book == null)
        {
            return NotFound();
        }

        return book;
    }

    [HttpPost]
    [Route("books")]
    public async Task<ActionResult<Book>> Postbook(Book book)
    {
        _context.books.Add(book);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Getbook), new { id = book.Id }, book);
    }

    [HttpGet]
    [Route("routes")]
    public IActionResult ListRoutes()
    {
        var endpointDataSource = HttpContext.RequestServices.GetService<EndpointDataSource>();

        var routes = endpointDataSource.Endpoints
            .Select(e => new
            {
                // RoutePattern = e.Metadata.GetMetadata<IPageRouteMetadata>()?.RoutePattern?.RawText,
                HttpMethod = e.Metadata.GetMetadata<HttpMethodMetadata>()?.HttpMethods?.FirstOrDefault(),
                EndpointName = e.DisplayName
            })
            .ToList();

        return Ok(routes);
    }

    [HttpPost("upload")]
    public async Task<IActionResult> Upload()
    {
        var files = Request.Form.Files;
        foreach (var file in files)
        {
            if (file == null || file.Length == 0)
            {
                continue;
            }

            var fileName = file.FileName;
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "View/books_frontend/public/BooksCovers", fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Save the file name to the database
            var book = new Book
            {
                ImageName = fileName,
            };

            _context.books.Add(book);
            _context.SaveChanges();
        }

        return Ok();
    }

    [HttpGet("id")]
    public Book GetId()
    {
        Book insertedRecord = _context.books.OrderByDescending(x => x.Id).First();
        LastRecordId = insertedRecord;
        return this.LastRecordId;
    }

    [HttpPut("books/{id}")]
    public async Task<IActionResult> Putbook(int id, Book book)
    {

        _context.Entry(book).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("books/{id}")]
    public async Task<IActionResult> Deletebook(int id)
    {
        var book = await _context.books.FindAsync(id);

        if (book == null)
        {
            return NotFound();
        }

        _context.books.Remove(book);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}


