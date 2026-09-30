using FluentValidation.TestHelper;
using Recon.Application.Tenants;

namespace Recon.UnitTests.Tenants;

public class TenantValidatorTests
{
    private static CreateTenantValidator CreateValidator() =>
        new(TestDbContextFactory.Create());     // validator needs the DB for the duplicate check

    [Fact]
    public async Task Valid_request_passes()
    {
        var validator = CreateValidator();
        var result = await validator.TestValidateAsync(
            new CreateTenantRequest("Test Name", "testcode")
        );

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Empty_name_fails()
    {
        var validator = CreateValidator();
        var result = await validator.TestValidateAsync(
            new CreateTenantRequest("", "testcode")
        );

        // FluentValidation's test helper: assert the error is on the Name field
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public async Task Code_with_capitals_or_spaces_fails()
    {
        var validator = CreateValidator();
        var result = await validator.TestValidateAsync(
            new CreateTenantRequest("Test Tenant Name", "Test Tenant Code")
        );

        result.ShouldHaveValidationErrorFor(x => x.Code);
    }
}