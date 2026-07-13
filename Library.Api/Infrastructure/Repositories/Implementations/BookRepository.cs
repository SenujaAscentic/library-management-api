using Library.Api.Domain.Entities;
using Library.Api.Infrastructure.Data;

public class BookRepository : IBookRepository
{
    private readonly LibraryDbContext _context;

    public BookRepository(LibraryDbContext context)
    {
        _context = context;
    }

    public async Task<List<Book>> GetAllBooksAsync()
    {
        return await _context.Books.ToListAsync();
    }

    public async Task<Book?> GetBookByIdAsync(Guid bookId)
    {
        return await _context.Books.FindAsync(bookId);
    }

    public async Task<Book?> GetBookByIsbnAsync(string isbn)
    {
        return await _context.Books.FirstOrDefaultAsync(b => b.Isbn == isbn);
    }

    public async Task AddBookAsync(Book book)
    {
        await _context.Books.AddAsync(book);
        await SaveChangesAsync();
    }

    public async Task UpdateBookAsync(Book book)
    {
        _context.Books.Update(book);
        await SaveChangesAsync();
    }

    public async Task DeleteBookAsync(Guid bookId)
    {
        var book = await GetBookByIdAsync(bookId);
        if (book != null)
        {
            _context.Books.Remove(book);
            await SaveChangesAsync();
        }
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}