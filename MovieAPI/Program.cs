using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MovieAPI.Data;
using MovieAPI.Services;


WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ActorContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ActorContext") ?? throw new InvalidOperationException("Connection string 'ActorContext' not found.")));
builder.Services.AddDbContext<MovieAPIContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MovieAPIContext") ?? throw new InvalidOperationException("Connection string 'MovieAPIContext' not found.")));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<MovieInfoRepository>();

builder.Services.AddAutoMapper(config => { }, AppDomain.CurrentDomain.GetAssemblies());

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
