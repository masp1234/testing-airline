using Neo4jClient;
using backend.Models.Neo4jModels;
using AutoMapper;
using backend.Models;

namespace backend.Repositories.Neo4j;
public class AirlineNeo4jRepository(IGraphClient graphClient, IMapper mapper): IAirlineRepository
{
    private readonly IGraphClient _graphClient = graphClient;
    private readonly IMapper _mapper = mapper;


    public async Task<List<Airline>> GetAll()
    {
        var query = await _graphClient.Cypher
            .Match("(a:Airline)")  // Match all nodes with the label 'Airline'
            .Return(a => a.As<Neo4jAirline>())  // Map the node to the Neo4jAirline model
            .ResultsAsync;

        var airlines = query.ToList();

        return _mapper.Map<List<Airline>>(airlines);;  
    }
}