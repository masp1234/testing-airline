using System.Text.Json;
using backend.Dtos;

namespace backend.Services
{
    public class DistanceApiService: IDistanceApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string? _apiKey;

        public DistanceApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://maps.googleapis.com/maps/api/distancematrix/json");
            _apiKey = Environment.GetEnvironmentVariable("GOOGLE_DISTANCE_API_KEY") ?? null;
        }
        public async Task<double?> GetDistanceData(string origin, string destination)
        {
            // If API key is not used, just return some default value (2000km). This is to avoid depleting the google cloud credits
            if (_apiKey == null)
            {
                return (2000000.00);
            }
            if (string.IsNullOrEmpty(origin) || string.IsNullOrEmpty(destination))
            {
                throw new ArgumentNullException("Origin and destination have to be not null and not be an empty string.");
            }
            var response = await _httpClient.GetAsync($"?destinations={destination}&origins={origin}&units=imperial&key={_apiKey}");

            // Throws an exception if the status code is not in the success range
            response.EnsureSuccessStatusCode();
            DistanceApiResponse? data = await response.Content.ReadFromJsonAsync<DistanceApiResponse>();

            if (data?.Rows == null || data.Rows.Count == 0 ||
                data.Rows[0].Elements == null || data?.Rows?[0]?.Elements?.Count == 0)
            {
                throw new InvalidOperationException("The DistanceApiResponse does not contain the expected data.");
            }
            return data?.Rows[0].Elements?[0]?.Distance?.Value;
        }
    }
}
