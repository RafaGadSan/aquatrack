using System.Net.Http.Json;
using System.Text.Json;
using AquaTrack.Application.Interfaces;
using AquaTrack.Domain.Entities;
using AquaTrack.Domain.Enums;
using AquaTrack.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace AquaTrack.Api.IntegrationTests;

/// <summary>
/// Swaps the Postgres DbContext for a SQLite in-memory one, so these tests don't need a real
/// Postgres instance (Testcontainers-style integration testing is left for when it earns its
/// setup cost). The connection is kept open for the factory's lifetime, since SQLite drops an
/// in-memory database as soon as its last connection closes.
///
/// Seeds one user per role so authorization tests can log in for real (POST /api/auth/login) and
/// exercise the actual JWT + [Authorize(Roles=...)] pipeline, instead of fabricating tokens.
/// </summary>
public class AquaTrackWebApplicationFactory : WebApplicationFactory<Program>
{
    public const string TestPassword = "TestPassword123!";
    public const string AdminEmail = "test.admin@aquatrack.dev";
    public const string ShiftLeadEmail = "test.shiftlead@aquatrack.dev";
    public const string OperatorEmail = "test.operator@aquatrack.dev";

    private readonly SqliteConnection _connection = new("DataSource=:memory:");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        // User secrets (where Jwt:Secret normally lives for local dev) are only loaded under the
        // Development environment, so the "Testing" environment needs its own explicit value.
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Secret"] = "integration-test-signing-key-not-for-real-use-only",
            });
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<AquaTrackDbContext>>();

            _connection.Open();
            services.AddDbContext<AquaTrackDbContext>(options => options.UseSqlite(_connection));

            using var scope = services.BuildServiceProvider().CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AquaTrackDbContext>();
            context.Database.EnsureCreated();

            var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
            context.Users.AddRange(
                new User(AdminEmail, passwordHasher.Hash(TestPassword), "Test Admin", Role.Admin),
                new User(ShiftLeadEmail, passwordHasher.Hash(TestPassword), "Test Shift Lead", Role.ShiftLead),
                new User(OperatorEmail, passwordHasher.Hash(TestPassword), "Test Operator", Role.Operator));
            context.SaveChanges();
        });
    }

    /// <summary>Logs in as the given seeded user and returns the JWT, for tests that need an authenticated client.</summary>
    public async Task<string> LoginAsync(HttpClient client, string email)
    {
        var response = await client.PostAsJsonAsync("/api/auth/login", new { email, password = TestPassword });
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        return body.GetProperty("token").GetString()!;
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            _connection.Dispose();
        }
    }
}
