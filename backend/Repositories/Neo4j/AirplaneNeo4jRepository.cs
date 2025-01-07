using Neo4jClient;
using backend.Models.Neo4jModels;
using AutoMapper;
using backend.Models;

namespace backend.Repositories.Neo4j;
public class AirplaneNeo4jRepository(IGraphClient graphClient, IMapper mapper): IAirplaneRepository
{
    private readonly IGraphClient _graphClient = graphClient;
    private readonly IMapper _mapper = mapper;


    public async Task<List<Airplane>> GetAll()
    {
        var query = await _graphClient.Cypher
            .Match("(a:Airplane)")  
            .Return(a => a.As<Neo4jAirplane>())  
            .ResultsAsync;

        var airplanes = query.ToList();

        return _mapper.Map<List<Airplane>>(airplanes);;  
    }

     public async Task<Airplane?> GetAirplaneById(long id)
    {
            var airplane = await _graphClient.Cypher
            .Match("(a:Airplane)")  
            .Where((Neo4jAirplane a) => a.Id == id)
            .Return(a => a.As<Neo4jAirplane>())  
            .ResultsAsync;

            var singleNeo4jairplane = airplane.SingleOrDefault();

            return singleNeo4jairplane == null ? null : _mapper.Map<Airplane>(singleNeo4jairplane);
            
    }
}