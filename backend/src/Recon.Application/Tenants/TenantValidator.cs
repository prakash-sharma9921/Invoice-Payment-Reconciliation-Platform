using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Recon.Application.Common.Interfaces;

namespace Recon.Application.Tenants;

// Checks the incoming request BEFORE we try to save anything.
public class CreateTenantValidator : AbstractValidator<CreateTenantRequest>
{
    public CreateTenantValidator(IApplicationDbContext db)
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(200);

        RuleFor(x => x.Code)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Code is required")
            .MaximumLength(50)
            .Matches("^[a-z0-9-]+$").WithMessage("Code can contain only lowercase letters, numbers, and hyphens.")
            .MustAsync(async (code, ct) =>
                !await db.Tenants.AnyAsync(t => t.Code == code, ct))
                .WithMessage("A tenant with this code already exists.");
    }
}