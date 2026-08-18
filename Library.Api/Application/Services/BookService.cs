

using Library.Api.Application.Interfaces;
using Library.Application.Abstractions.Repositories;
using Library.Application.Features.Books;
using Library.Domain.Exceptions;
using Library.Api.Contracts.Books;
namespace Library.Api.Application.Services;
public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;

    public BookService(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }
    
    
   

    
    public async Task UpdateAsync(Guid id, UpdateBookRequest request)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        if (book is null)
        {
            throw new NotFoundException("Book not found.");
        }
        var duplicateBook = await _bookRepository.GetByIsbnAsync(request.Isbn);
        if (duplicateBook is not null && book.Id != id)
        {
            throw new ConflictException("ISBN already exists.");
        }

        book.Title = request.Title;
        book.Author = request.Author;
        book.Isbn = request.Isbn;
        book.PublishedYear = request.PublishedYear;
        book.TotalCopies = request.TotalCopies;
        await _bookRepository.SaveChangesAsync();
    }
    public async Task DeleteAsync(Guid id)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        if (book is null)
        {
            throw new NotFoundException("Book not found.");
        }
        _bookRepository.Delete(book);
        await _bookRepository.SaveChangesAsync();
    }
}