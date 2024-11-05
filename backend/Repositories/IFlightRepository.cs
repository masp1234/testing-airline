namespace backend.Repositories
{
    public interface IFlightRepository
    {
        public Task Create(Flight flight);
    }
}
