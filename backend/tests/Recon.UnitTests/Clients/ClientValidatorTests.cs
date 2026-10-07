using FluentValidation.TestHelper;
using Recon.Application.Clients;
using Recon.Domain.Entities;

namespace Recon.UnitTests.Clients;

public class ClientValidatorTests
{
    private static CreateClientValidator CreateValidator() =>
        new(TestDbContextFactory.Create());

    [Fact]
    public async Task Invalid_Email_Fails()
    {
        var result = await CreateValidator().TestValidateAsync(
            new CreateClientRequest("Prakash", "prakash", null, ClientType.Customer)
        );

        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public async Task Valid_Request_Passes()
    {
        var result = await CreateValidator().TestValidateAsync(
            new CreateClientRequest("Prakash Sharma", "prakash@gmail.com", null, ClientType.Customer)
        );

        result.ShouldNotHaveAnyValidationErrors();
    }
}