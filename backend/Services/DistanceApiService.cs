using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;

namespace backend.Services
{
    public class DistanceApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string? _apiKey;

        public DistanceApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://maps.googleapis.com/maps/api/distancematrix/json");
            _apiKey = Environment.GetEnvironmentVariable("GOOGLE_DISTANCE_API_KEY") ?? null;
        }
        public async Task<string> GetDistanceData(string origin, string destination)
        {
            var response = await _httpClient.GetAsync($"?destinations={destination}&origins={origin}&units=imperial&key={_apiKey}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}
