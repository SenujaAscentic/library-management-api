using MediatR;
using Microsoft.Extensions.Logging;
using Library.Domain.Entities;
using Library.Domain.Exceptions;
using Library.Application.Abstractions.Repositories;
using Library.Application.Abstractions.Messaging;

namespace Library.Application.Features.Books.Commands.CreateBook;

public class CreateBookCommandHandler(IBookRepository bookRepository, ILogger<CreateBookCommandHandler> logger)
    : ICommandHandler<CreateBookCommand, BookResponse>
{
    public async Task<BookResponse> Handle(CreateBookCommand command, CancellationToken cancellationToken)
    {
        var existingBook = await bookRepository.GetByIsbnAsync(command.Isbn);
        if (existingBook is not null)
        {
            logger.LogWarning("Create book rejected: ISBN {Isbn} already exists", command.Isbn);
            throw new ConflictException("ISBN already exists.");
        }

        var book = new Book
        {
            Id = Guid.NewGuid(),
            Title = command.Title,
            Author = command.Author,
            Isbn = command.Isbn,
            PublishedYear = command.PublishedYear,
            TotalCopies = command.TotalCopies,
            AvailableCopies = command.TotalCopies
        };

        await bookRepository.AddAsync(book);
        await bookRepository.SaveChangesAsync();

        logger.LogInformation("Book created: {BookId} with ISBN {Isbn}", book.Id, book.Isbn);

        return new BookResponse(book.Id, book.Title, book.Author, book.Isbn, book.PublishedYear, book.TotalCopies, book.AvailableCopies);
    }
}