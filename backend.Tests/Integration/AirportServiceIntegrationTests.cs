using AutoMapper;
using backend.Services;
using backend.Config;
using backend.Repositories;
using backend.Models;

namespace backend.Tests.Integration
{
    public class AirportServiceIntegrationTests: IClassFixture<TestDatabaseFixture>
    {
        private readonly IAirportService _sut;
        private readonly TestDatabaseFixture _dbFixture;

        public AirportServiceIntegrationTests(TestDatabaseFixture dbFixture)
        {
            _dbFixture = dbFixture;
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });

            IMapper mapper = configuration.CreateMapper();
            _sut = new AirportService(new AirportRepository(_dbFixture.DbContext), mapper);

            _dbFixture.ResetDatabase();
        }

        [Fact]
        public async Task GetAllAirports_ShouldReturn_Airports()
        {
            // Arrange
            await _dbFixture.DbContext.Airports.AddAsync(new Airport { Name = "Test airport", Code = "TEST" });
            await _dbFixture.DbContext.SaveChangesAsync();

            // Act
            var airports = await _sut.GetAllAirports();

            // Assert
            Assert.NotEmpty(airports);
        }

        [Fact]
        public async Task GetAllAirports_ShouldReturnEmptyList_WhenNoAirportsFound()
        {
            var airports = await _sut.GetAllAirports();

            // Assert
            Assert.Empty(airports);
        }
    }

    
}
