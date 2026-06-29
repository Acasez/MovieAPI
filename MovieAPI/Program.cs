using Microsoft.EntityFrameworkCore;
using MovieAPI.Data;
using MovieAPI.Interfaces;
using MovieAPI.Services;
using Newtonsoft.Json.Serialization;


WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<MovieAPIContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MovieAPIContext") ?? throw new InvalidOperationException("Connection string 'MovieAPIContext' not found.")));

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddLogging();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddScoped<IMovieService, MovieInfoRepository>();

builder.Services.AddAutoMapper(config => { }, AppDomain.CurrentDomain.GetAssemblies());

//builder.Services.AddControllers().AddJsonOptions(options =>{});
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        // Enable JSON Patch support for Newtonsoft.Json
        options.SerializerSettings.ContractResolver = new DefaultContractResolver();
    });
WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });
}


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
