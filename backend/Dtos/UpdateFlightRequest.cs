using backend.Models;
using System.Security.Claims;

namespace backend.Dtos
{
    public class UpdateFlightRequest
    {
        public DateTime DepartureDateTime { get; set; }

        public string? UpdatedBy { get; set; }
    }
}
