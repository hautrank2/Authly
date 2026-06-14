using Authly.Filters;
using Authly.Middlewares;
using Authly.Models;
using Authly.Services;
using Authly.Services.Dtos;
using MongoDB.Driver;


var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetSection("AuthlyDatabase:ConnectionString").Value;

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ApiResponseFilter>();
});
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add services to the container.
builder.Services.Configure<AuthlyDatabaseSettings>(
    builder.Configuration.GetSection("AuthlyDatabase"));

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddSingleton<IMongoClient>(_ => new MongoClient(connectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseGlobalExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
