using Library.Api.Domain.Entities;
using Library.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Library.Api.Infrastructure.Repositories.Interfaces;

namespace Library.Api.Infrastructure.Repositories.Implementations;

public class BookRepository : IBookRepository
{
    private readonly LibraryDbContext _db;

    public BookRepository(LibraryDbContext db)
    {
        _db = db;
    }

    public async Task<List<Book>> GetAllAsync()
    {
        return await _db.Books.ToListAsync();
    }

    public async Task<Book?> GetByIdAsync(Guid bookId)
    {
        return await _db.Books.FirstOrDefaultAsync(b => b.Id == bookId);
    }

    public async Task<Book?> GetByIsbnAsync(string isbn)
    {
        return await _db.Books.FirstOrDefaultAsync(b => b.Isbn == isbn);
    }

    public async Task AddAsync(Book book)
    {
        await _db.Books.AddAsync(book);
        
    }

    public void Update(Book book)
    {
        _db.Books.Update(book);
        
    }

    public void Delete(Book book)
    {
        _db.Books.Remove(book);
    }

    public async Task SaveChangesAsync()
    {
        await _db.SaveChangesAsync();
    }
}