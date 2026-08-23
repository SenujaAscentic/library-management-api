using FluentValidation;


namespace Library.Application.Features.Books.Commands.CreateBook;

public class CreateBookCommandValidator : AbstractValidator<CreateBookCommand>
{
    public CreateBookCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty();

        RuleFor(x => x.Author)
            .NotEmpty();
        
        RuleFor(x => x.Isbn)
            .NotEmpty();
        
        RuleFor(x => x.TotalCopies)
            .GreaterThan(0);
        
        RuleFor(x => x.PublishedYear)
            .LessThanOrEqualTo(DateTime.UtcNow.Year);
    }
}