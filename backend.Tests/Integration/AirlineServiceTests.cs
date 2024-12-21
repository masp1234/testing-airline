using AutoMapper;
using backend.Config;
using backend.Database;
using backend.Repositories;
using backend.Services;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MySql;
using Xunit.Abstractions;

public class AirlineServiceTests(ITestOutputHelper output) : IAsyncLifetime
{
    private AirlineService _sut;
    private readonly MySqlContainer _mySqlContainer = new MySqlBuilder()
        .WithImage("mysql:8.0")
        .WithDatabase("airline_project")
    .WithUsername("testuser")
    .WithPassword("123123")
        .WithCommand("--default-authentication-plugin=mysql_native_password")
    .Build();

    private DatabaseContext _dbContext;
    private readonly ITestOutputHelper _output = output;

    public async Task DisposeAsync()
    {
        await _dbContext.DisposeAsync();
        await _mySqlContainer.DisposeAsync();
    }

    [Fact]
    public async Task GetAllAirlines_ShouldReturn_Airlines()
    {
        var airlines = await _sut.GetAllAirlines();
        Assert.NotEmpty(airlines);
    }

    public async Task InitializeAsync()
    {
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new MappingProfile());
        });

        await _mySqlContainer.StartAsync();

        var connectionString = $"Server={_mySqlContainer.Hostname};Port={_mySqlContainer.GetMappedPublicPort(3306)};Database=airline_project;Uid=root;Pwd=123123;";

        _output.WriteLine(connectionString);

        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), options =>
            {
                options.EnableRetryOnFailure(maxRetryCount: 2);
            })
            .Options;

        _dbContext = new DatabaseContext(options);

        await _dbContext.Database.EnsureCreatedAsync();

        _output.WriteLine("DATABASE CREATED?");
        IMapper mapper = configuration.CreateMapper();
        _sut = new AirlineService(new AirlineRepository(_dbContext), mapper);
    }
}
