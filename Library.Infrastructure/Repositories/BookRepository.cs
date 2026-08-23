using Library.Domain.Entities;
using Library.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Library.Application.Abstractions.Repositories;

namespace Library.Infrastructure.Repositories;

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

    
}