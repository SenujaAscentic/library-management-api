
using Library.Api.Contracts.Books;
using Library.Application.Features.Books;

namespace Library.Api.Application.Interfaces;

public interface IBookService
{
    
    
    Task UpdateAsync(Guid id, UpdateBookRequest request);
    Task DeleteAsync(Guid id);
}
