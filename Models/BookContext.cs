using Microsoft.EntityFrameworkCore;
namespace Book.Models;
public class BookContext : DbContext
{

    public BookContext(DbContextOptions<BookContext> options) : base(options)
    {

    }
    public DbSet<Book> books { get; set; }
}
