using backend.Database;
using backend.Models;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MySql;

public class TestDatabaseFixture : IAsyncLifetime
{
    private readonly MySqlContainer _container;

    public DatabaseContext DbContext { get; private set; }

    public TestDatabaseFixture()
    {
        _container = new MySqlBuilder()
            .WithImage("mysql:8.0")
            .Build();
    }

    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        var connectionString = _container.GetConnectionString();

        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), options =>
            {
                options.EnableRetryOnFailure(maxRetryCount: 3);
            })
            .Options;

        DbContext = new DatabaseContext(options);
        await DbContext.Database.EnsureCreatedAsync();
    }

    public void ResetDatabase()
    {
        DbContext.Database.EnsureDeleted();
        DbContext.Database.EnsureCreated();
    }

    public async Task DisposeAsync()
    {
        await DbContext.DisposeAsync();
        await _container.DisposeAsync();
    }
}
