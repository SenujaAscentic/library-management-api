

using Library.Api.Application.Interfaces;
using Library.Api.Contracts.Books;
using Library.Api.Domain.Entities;
using Library.Api.Infrastructure.Repositories.Interfaces;

namespace Library.Api.Application.Services;
public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;

    public BookService(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }
    
    public async Task<BookResponse> CreateAsync(CreateBookRequest request)
    {
        var existingBook = await _bookRepository.GetByIsbnAsync(request.Isbn);
        if (existingBook is not null)
        {
            throw new Exception("ISBN already exists.");
        }
        var book = new Book
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Author = request.Author,
            Isbn = request.Isbn,
            PublishedYear = request.PublishedYear,
            TotalCopies = request.TotalCopies,
            AvailableCopies = request.TotalCopies
        };
        await _bookRepository.AddAsync(book);
        await _bookRepository.SaveChangesAsync();
        return new BookResponse(
            book.Id,
            book.Title,
            book.Author,
            book.Isbn,
            book.PublishedYear,
            book.TotalCopies,
            book.AvailableCopies);
    }

    public async Task<List<BookResponse>> GetAllAsync()
    {
        var books = await _bookRepository.GetAllAsync();
        return books.Select(book => new BookResponse(
            book.Id,
            book.Title,
            book.Author,
            book.Isbn,
            book.PublishedYear,
            book.TotalCopies,
            book.AvailableCopies)).ToList();
    }

    public async Task<BookResponse?> GetByIdAsync(Guid id)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        if (book is null)
        {
            return null;
        }
        return new BookResponse(
            book.Id,
            book.Title,
            book.Author,
            book.Isbn,
            book.PublishedYear,
            book.TotalCopies,
            book.AvailableCopies);
    }
    public async Task UpdateAsync(Guid id, UpdateBookRequest request)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        if (book is null)
        {
            throw new Exception("Book not found.");
        }
        var duplicateBook = await _bookRepository.GetByIsbnAsync(request.Isbn);
        if (duplicateBook is not null && book.Id != id)
        {
            throw new Exception("ISBN already exists.");
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
            throw new Exception("Book not found.");
        }
        _bookRepository.Delete(book);
        await _bookRepository.SaveChangesAsync();
    }
}