using backend.Database;
using backend.Models;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MySql;
using Xunit.Abstractions;
using System.Collections.Generic;
using System.Linq;
using Xunit.Abstractions;
using Xunit.Sdk;

public class TestDatabaseFixture : IAsyncLifetime
{
    private readonly MySqlContainer _container;

    public DatabaseContext DbContext { get; private set; }
    private readonly IMessageSink diagnosticMessageSink;

    public TestDatabaseFixture(IMessageSink logger)
    {
        diagnosticMessageSink = logger;
        _container = new MySqlBuilder()
            .WithImage("mysql:8.0")
            .WithDatabase("airline_project_tests")
            .WithPassword("123123")
            .WithUsername("testuser")
            .WithPortBinding(3306)
            .WithCleanUp(true)
            .Build();
    }

    public async Task InitializeAsync()
    {
        // TRY THIS https://xunit.net/docs/configuration-files#diagnosticMessages - ADD CONFIG FILE
        var message = new DiagnosticMessage("Ordered {0} test cases", "test");
        diagnosticMessageSink.OnMessage(message);
        await _container.StartAsync();

        var connectionString = _container.GetConnectionString();
        TestLogger.Log(connectionString);

        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), options =>
            {
                options.EnableRetryOnFailure(maxRetryCount: 5);
            })
            .Options;

        DbContext = new DatabaseContext(options);
        await SeedDatabase();
    }

    private async Task SeedDatabase()
    {
        await DbContext.Airlines.AddAsync(new Airline { Name = "Test airline" });
        await DbContext.SaveChangesAsync();
    }

    public async Task DisposeAsync()
    {
        await DbContext.DisposeAsync();
        await _container.DisposeAsync();
    }
}
