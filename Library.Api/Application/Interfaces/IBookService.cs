

using Library.Application.Features.Books;

namespace Library.Api.Application.Interfaces;

public interface IBookService
{
    
    
    
    Task DeleteAsync(Guid id);
}
