using FluentValidation;
using Library.Api.Application.contracts.Members;
using Library.Api.Contracts.Members;

namespace Library.Api.Validators;

public class UpdateMemberRequestValidator
    : AbstractValidator<UpdateMemberRequest>
{
    public UpdateMemberRequestValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty();

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.PhoneNumber)
            .NotEmpty();
    }
}