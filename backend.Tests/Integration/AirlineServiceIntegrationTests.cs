using AutoMapper;
using backend.Config;
using backend.Database;
using backend.Models;
using backend.Repositories;
using backend.Services;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MySql;
using Xunit.Abstractions;

public class AirlineServiceIntegrationTests: IClassFixture<TestDatabaseFixture>
{
    private readonly AirlineService _sut;
    private readonly TestDatabaseFixture _dbFixture;

    public AirlineServiceIntegrationTests(TestDatabaseFixture dbFixture)
    {
        _dbFixture = dbFixture;
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new MappingProfile());
        });

        IMapper mapper = configuration.CreateMapper();
        _sut = new AirlineService(new AirlineRepository(_dbFixture.DbContext), mapper);

        _dbFixture.ResetDatabase();
    }

    [Fact]
    public async Task GetAllAirlines_ShouldReturn_Airlines()
    {
        // Arrange
        await _dbFixture.DbContext.Airlines.AddAsync(new Airline { Name = "Test airline" });
        await _dbFixture.DbContext.SaveChangesAsync();

        // Act
        var airlines = await _sut.GetAllAirlines();

        // Assert
        Assert.NotEmpty(airlines);
    }

    [Fact]
    public async Task GetAllAirlines_ShouldReturnEmptyList_WhenNoAirlinesFound()
    {
        var airlines = await _sut.GetAllAirlines();

        // Assert
        Assert.Empty(airlines);
    }
}
