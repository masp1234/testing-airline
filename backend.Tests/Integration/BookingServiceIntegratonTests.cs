using backend.Services;
using Moq;

namespace backend.Tests.Integration
{
    public class BookingServiceIntegrationTests: IClassFixture<TestDatabaseFixture>
    {
        private readonly IBookingService _bookingService;
        private readonly Mock<IFlightService> _mockFlightService;
        private readonly Mock<IUserService> _mockUserService ;
        private readonly Mock<IEmailService> _mockEmailService;
        private readonly Mock<ITicketAvailabilityChecker> _mockTicketAvailabilityChecker;

    }
}