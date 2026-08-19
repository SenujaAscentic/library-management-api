using FluentValidation;

namespace Library.Application.Features.Books.Commands.UpdateBook;

    public class UpdateBookCommandValidator : AbstractValidator<UpdateBookCommand>
    {
         public UpdateBookCommandValidator()
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

