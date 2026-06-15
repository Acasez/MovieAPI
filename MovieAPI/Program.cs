using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MovieAPI.Data;
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<MovieAPIContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MovieAPIContext") ?? throw new InvalidOperationException("Connection string 'MovieAPIContext' not found.")));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
