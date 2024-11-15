namespace backend.Services
{
    public interface IDistanceApiService
    {
        Task<double?> GetDistanceData(string origin, string destination);
    }
}
