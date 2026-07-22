using System.Text;
using AquaTrack.Api.Middleware;
using AquaTrack.Application;
using AquaTrack.Infrastructure;
using AquaTrack.Infrastructure.Persistence;
using AquaTrack.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()));

// Keep validation messages in English regardless of the host's OS locale, so behavior doesn't
// silently differ between a developer's machine and the (Linux, English-locale) deployed container.
FluentValidation.ValidatorOptions.Global.LanguageManager.Enabled = false;

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your JWT token}",
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
            Array.Empty<string>()
        },
    });
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Without this, every request from the frontend (a different origin — localhost:5173 vs this API's
// localhost:5000) is silently blocked by the browser before it even reaches the controllers. curl
// and integration tests don't enforce CORS, so this gap went unnoticed until a real headless-browser
// check surfaced "net::ERR_FAILED" on login. Single configurable origin is enough for this project's
// shape (one frontend deployment); revisit if that ever needs to be a list.
var allowedOrigin = builder.Configuration["Cors:AllowedOrigin"] ?? "http://localhost:5173";
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
        policy.WithOrigins(allowedOrigin)
            .AllowAnyHeader()
            .AllowAnyMethod());
});

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer();

// Configured via IOptions rather than a snapshot read off builder.Configuration: the latter is
// evaluated before builder.Build() finishes composing all configuration sources (including, for
// integration tests, WebApplicationFactory's in-memory overrides), so it would silently miss them.
builder.Services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
    .Configure<IOptions<JwtSettings>>((bearerOptions, jwtOptions) =>
    {
        var settings = jwtOptions.Value;
        bearerOptions.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = settings.Issuer,
            ValidAudience = settings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Secret)),
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

// Auto-apply migrations and seed demo data on startup. This is a deliberate simplification for a
// portfolio demo (single instance, no deploy pipeline) — a real production setup would run
// migrations as a separate release step, not at app boot. Skipped under "Testing" so integration
// tests can swap in their own (SQLite) DbContext without this racing against a Postgres migration.
if (!app.Environment.IsEnvironment("Testing"))
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<AquaTrackDbContext>();
    await context.Database.MigrateAsync();

    var passwordHasher = scope.ServiceProvider.GetRequiredService<AquaTrack.Application.Interfaces.IPasswordHasher>();
    await DbInitializer.SeedAsync(context, passwordHasher);
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("Frontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program
{
}
