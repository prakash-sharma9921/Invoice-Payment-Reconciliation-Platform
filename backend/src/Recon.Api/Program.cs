using Microsoft.EntityFrameworkCore;
using Recon.Infrastructure.Presistance;

var builder = WebApplication.CreateBuilder(args);

// Connect Database
builder.Services.AddDbContext<ReconDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ReconDb")));

builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();
