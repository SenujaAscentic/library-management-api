using FluentValidation;
using Library.Api.Contracts.Borrowings;

namespace Library.Api.Validators;

public class BorrowBookRequestValidator
    : AbstractValidator<BorrowBookRequest>
{
    public BorrowBookRequestValidator()
    {
        RuleFor(x => x.BookId)
            .NotEmpty();

        RuleFor(x => x.MemberId)
            .NotEmpty();
    }
}