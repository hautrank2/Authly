using Authly.Filters;
using Authly.Middlewares;
using Authly.Models;
using Authly.Services;
using Authly.Services.Dtos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using StackExchange.Redis;
using System.Text;


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
builder.Services.Configure<CloudinarySettings>(
    builder.Configuration.GetSection("Cloudinary"));

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var redisConnString = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379";
    return ConnectionMultiplexer.Connect(redisConnString);
});

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

// Config authentication
var jwtKey = builder.Configuration.GetSection("Jwt:Key").Value
             ?? throw new InvalidOperationException("JWT Key is not configured.");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration.GetSection("Jwt:Issuer").Value,
        ValidAudience = builder.Configuration.GetSection("Jwt:Audience").Value,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.Zero,
    };

    // Kiểm tra token có trong blacklist không
    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = async context =>
        {
            try
            {
                var jti = context.Principal?.FindFirst("jti")?.Value;
                if (string.IsNullOrEmpty(jti))
                {
                    Console.WriteLine("[JWT] FAIL: Token missing jti claim.");
                    context.Fail("Token missing jti claim.");
                    return;
                }

                var redis = context.HttpContext.RequestServices.GetRequiredService<IConnectionMultiplexer>();
                var db = redis.GetDatabase();
                var isRevoked = await db.KeyExistsAsync($"revoked_token:{jti}");

                if (isRevoked)
                {
                    Console.WriteLine("[JWT] FAIL: Token has been revoked.");
                    context.Fail("Token has been revoked.");
                }
                else
                {
                    Console.WriteLine($"[JWT] OK: jti={jti}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[JWT] ERROR in OnTokenValidated: {ex.Message}");
            }
        },
        OnChallenge = context =>
        {
            Console.WriteLine($"[JWT] OnChallenge (401): {context.Error} - {context.ErrorDescription}");
            return Task.CompletedTask;
        },
        OnForbidden = context =>
        {
            Console.WriteLine("[JWT] OnForbidden (403)");
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddSingleton<IMongoClient>(_ => new MongoClient(connectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseGlobalExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
