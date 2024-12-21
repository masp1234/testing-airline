﻿using AutoMapper;
using backend.Config;
using backend.Database;
using backend.Models;
using backend.Repositories;
using backend.Services;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MySql;
using Xunit.Abstractions;

public class AirlineServiceTests: IClassFixture<TestDatabaseFixture>
{
    private readonly AirlineService _sut;
    private readonly TestDatabaseFixture _dbFixture;

    public AirlineServiceTests(TestDatabaseFixture dbFixture)
    {
        _dbFixture = dbFixture;
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new MappingProfile());
        });

        IMapper mapper = configuration.CreateMapper();
        _sut = new AirlineService(new AirlineRepository(_dbFixture.DbContext), mapper);
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
}