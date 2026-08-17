using Library.Domain.Entities;

namespace Library.Application.Abstractions.Repositories;
public interface IBookRepository
{
    Task<List<Book>> GetAllAsync();
    Task<Book?> GetByIdAsync(Guid bookId);
    Task<Book?> GetByIsbnAsync(string isbn);
    Task AddAsync(Book book);       
    void Update(Book book);
    void Delete(Book book);
    Task SaveChangesAsync();
}