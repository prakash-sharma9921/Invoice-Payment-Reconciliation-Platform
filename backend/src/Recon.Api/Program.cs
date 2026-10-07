using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Recon.Api.Endpoints;
using Recon.Api.Tenancy;
using Recon.Application.Clients;
using Recon.Application.Common.Interfaces;
using Recon.Application.Tenants;
using Recon.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Connect Database
builder.Services.AddDbContext<ReconDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ReconDb")));

// Let the Application's interface resolve to the real DbContext (same instance per request)
builder.Services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ReconDbContext>());

// Register our tenant logic
builder.Services.AddScoped<ITenantService, TenantService>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddHttpContextAccessor();      // enables reading the current request
builder.Services.AddScoped<ICurrentTenant, HeaderCurrentTenant>();      // "when someone needs ICurrentTenant, give them this"

// Find and register every FluentValidation validator in the Application project
builder.Services.AddValidatorsFromAssemblyContaining<CreateTenantRequest>();

builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapTenantEndpoints();
app.MapClientEndpoints();
app.Run();

// Makes the auto-generated Program class visible to the integration test project.
public partial class Program { }