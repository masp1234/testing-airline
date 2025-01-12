using AutoMapper;
using backend.Config;
using backend.Dtos;
using backend.Models;
using backend.Repositories;
using backend.Services;
using Moq;


namespace backend.Tests.Unit
{
    public class BookingServiceUnitTests
    {
        private readonly IBookingService _bookingService;
        private readonly Mock<IFlightService> _mockFlightService;
        private readonly Mock<IUserService> _mockUserService ;
        private readonly Mock<IEmailService> _mockEmailService;
        private readonly Mock<IBookingRepository> _mockBookingRepository;
        private readonly Mock<ITicketAvailabilityChecker> _mockTicketAvailabilityChecker;

        private readonly List<Booking> _mockBookings = 
        [
            new()
            {
                Id = 1,
                ConfirmationNumber = "ABC1",
                UserId = 1
            },
            new()
            {
                Id = 1,
                ConfirmationNumber = "ABC2",
                UserId = 1 
            }
        ];
        private readonly List<UserResponse> _mockUsers = 
        [
            new()
            {
                Id = 1, 
                Email = "test1@example.com"
            },
            new()
            {
                Id = 2, 
                Email = "test2@example.com"
            },
            new()
            {
                Id = 3, 
                Email = "test3@example.com"
            },
            new()
            {
                Id = 4, 
                Email = "test4@example.com"
            },
        ];

        public BookingServiceUnitTests()
        {
            _mockFlightService = new Mock<IFlightService>();
            _mockEmailService = new Mock<IEmailService>();
            _mockBookingRepository = new Mock<IBookingRepository>();
            _mockTicketAvailabilityChecker = new Mock<ITicketAvailabilityChecker>();
            _mockUserService = new Mock<IUserService>();

            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });

            IMapper mapper = configuration.CreateMapper();

            _bookingService = new BookingService(_mockUserService.Object, _mockFlightService.Object, _mockEmailService.Object, _mockBookingRepository.Object, mapper,  _mockTicketAvailabilityChecker.Object );


        }

        [Fact]
        public async Task GetBookingsByUserEmail_ReturnsSuccess_WithBookings()
        {
            string email = "test1@example.com";
        

            _mockUserService.Setup(service => service.GetUserByEmail(email)).ReturnsAsync(_mockUsers[0]);
            _mockBookingRepository.Setup(repo => repo.GetBookingsByUserId(_mockUsers[0].Id)).ReturnsAsync(_mockBookings);

            var bokkings = await _bookingService.GetBookingsByUserEmail(email);

            Assert.True(bokkings.IsSucces);
            Assert.NotNull(bokkings.Data);
            Assert.Equal(2, bokkings.Data.Count);
            Assert.Equal("ABC1", bokkings.Data[0].ConfirmationNumber);
            Assert.Equal("ABC2", bokkings.Data[1].ConfirmationNumber);
        }

        [Fact]
        public async Task GetBookingsByUserEmail_ReturnsFailure_WhenUserNotFound()
        {
            string email = "test@example.com";
            _mockUserService.Setup(service => service.GetUserByEmail(email))
                            .ReturnsAsync((UserResponse?)null);

            var bokkings = await _bookingService.GetBookingsByUserEmail(email);

            Assert.False(bokkings.IsSucces);
            Assert.Equal($"No user found with the email {email}", bokkings.Message);
            Assert.Null(bokkings.Data);
        }

        [Fact]
        public async Task GetBookingsByUserEmail_ReturnsSuccess_WithEmptyBookingsList()
        {
            string email = "test2@example.com";
            var userResponse = new UserResponse { Id = 1, Email = email };

            _mockUserService.Setup(service => service.GetUserByEmail(email))
                            .ReturnsAsync(_mockUsers[1]);

            _mockBookingRepository.Setup(repo => repo.GetBookingsByUserId(userResponse.Id))
                                .ReturnsAsync(new List<Booking>());

            var result = await _bookingService.GetBookingsByUserEmail(email);

            Assert.True(result.IsSucces);
            Assert.NotNull(result.Data);
            Assert.Empty(result.Data);
        }

    }
}