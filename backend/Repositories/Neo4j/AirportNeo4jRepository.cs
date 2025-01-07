using Neo4jClient;
using backend.Models.Neo4jModels;
using AutoMapper;
using backend.Models;
using MongoDB.Bson;

namespace backend.Repositories.Neo4j;
public class AirportNeo4jRepository(IGraphClient graphClient, IMapper mapper): IAirportRepository
{
    private readonly IGraphClient _graphClient = graphClient;
    private readonly IMapper _mapper = mapper;


    public async Task<List<Airport>> GetAll()
    {
        var query = await _graphClient.Cypher
            .Match("(a:Airport)")  
            .Return(a => a.As<Neo4jAirport>())  
            .ResultsAsync;

        var airports = query.ToList();

        return _mapper.Map<List<Airport>>(airports);;  
    }

    public async Task<List<Airport>> FindByIds(params long[] ids)
        {
            var query = await _graphClient.Cypher
            .Match("(a:Airport)")  // Match Airport nodes
            .Where("a.id IN $ids") // Use the IN operator to check if 'a.Id' is in the provided
            .WithParam("ids", ids)  // Pass the array of ids as a parameter
            .Return(a => a.As<Neo4jAirport>())  // Return matched Airports
            .ResultsAsync;

            var airports = query.ToList();

            return _mapper.Map<List<Airport>>(airports);
        }
}
