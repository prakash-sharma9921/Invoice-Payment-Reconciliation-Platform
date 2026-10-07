using FluentValidation;
using Recon.Application.Common.Interfaces;
using Recon.Domain.Entities;

namespace Recon.Application.Clients;

public class CreateClientValidator : AbstractValidator<CreateClientRequest>
{
    public CreateClientValidator(IApplicationDbContext db)
    {
        RuleFor(c => c.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(200);

        RuleFor(c => c.Email)
            .MaximumLength(256)
            .EmailAddress().WithMessage("Email is not valid")
            .When(c => !string.IsNullOrEmpty(c.Email));

        RuleFor(c => c.Phone)
            .MaximumLength(50)
            .Matches(@"^[0-9+\-\s()]+$").WithMessage("Phone can contain only numbers, spaces and + - ( )")
            .When(c => !string.IsNullOrEmpty(c.Phone));

        RuleFor(c => c.Type)
            .IsInEnum().WithMessage("Client Type is invalid");
    }
}