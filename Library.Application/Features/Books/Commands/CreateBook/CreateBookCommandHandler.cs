using MediatR;
using Microsoft.Extensions.Logging;
using Library.Domain.Entities;
using Library.Domain.Exceptions;
using Library.Application.Abstractions.Repositories;
using Library.Application.Abstractions.Messaging;
using Library.Application.Abstractions;

namespace Library.Application.Features.Books.Commands.CreateBook;

public class CreateBookCommandHandler(IBookRepository bookRepository,IUnitOfWork unitOfWork, ILogger<CreateBookCommandHandler> logger)
    : ICommandHandler<CreateBookCommand, BookResponse>
{
    public async Task<BookResponse> Handle(CreateBookCommand command, CancellationToken cancellationToken)
    {
        var existingBook = await bookRepository.GetByIsbnAsync(command.Isbn);
        if (existingBook is not null)
        {
            logger.LogWarning("Create book rejected: ISBN {Isbn} already exists", command.Isbn);
            throw new ConflictException("duplicate_isbn", "The provided ISBN is already in use.");
        }

        var book = Book.Create(command.Title, command.Author, command.Isbn, command.PublishedYear, command.TotalCopies);

        await bookRepository.AddAsync(book);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Book created: {BookId} with ISBN {Isbn}", book.Id, book.Isbn);

        return new BookResponse(book.Id, book.Title, book.Author, book.Isbn, book.PublishedYear, book.TotalCopies, book.AvailableCopies);
    }
}