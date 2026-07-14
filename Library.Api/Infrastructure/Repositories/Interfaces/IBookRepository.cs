using Library.Api.Domain.Entities;

namespace Library.Api.Infrastructure.Repositories.Interfaces;
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