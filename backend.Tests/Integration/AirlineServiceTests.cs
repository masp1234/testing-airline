using AutoMapper;
using backend.Config;
using backend.Database;
using backend.Repositories;
using backend.Services;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MySql;
using Xunit.Abstractions;

public class AirlineServiceTests() : IAsyncLifetime
{
    private AirlineService _sut;
    private readonly MySqlContainer _mySqlContainer = new MySqlBuilder()
        .WithImage("mysql:8.0")
        .Build();

    private DatabaseContext _dbContext;

    public async Task DisposeAsync()
    {
        await _dbContext.DisposeAsync();
        await _mySqlContainer.DisposeAsync();
    }

    [Fact]
    public async Task GetAllAirlines_ShouldReturn_Airlines()
    {
        var airlines = await _sut.GetAllAirlines();
        Assert.Empty(airlines);
    }

    public async Task InitializeAsync()
    {
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new MappingProfile());
        });

        await _mySqlContainer.StartAsync();

        var connectionString = _mySqlContainer.GetConnectionString();

        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), options =>
            {
                options.EnableRetryOnFailure(maxRetryCount: 3);
            })
            .Options;

        _dbContext = new DatabaseContext(options);
        await _dbContext.Database.EnsureCreatedAsync();

        IMapper mapper = configuration.CreateMapper();
        _sut = new AirlineService(new AirlineRepository(_dbContext), mapper);
    }
}
