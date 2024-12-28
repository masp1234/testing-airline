using Neo4jClient;
using backend.Models.Neo4jModels;
using AutoMapper;
using backend.Models;
using backend.Utils;
using MongoDB.Bson;

namespace backend.Repositories.Neo4j;
public class UserNeo4jRepository(IGraphClient graphClient, IMapper mapper): IUserRepository
{
    private readonly IGraphClient _graphClient = graphClient;
    private readonly IMapper _mapper = mapper;


    public async Task Create(User user)
    {
         
        if (user == null) throw new ArgumentNullException(nameof(user));

        var userNeo4j = _mapper.Map<Neo4jUser>(user);

        //the id generator has to be fixed
        userNeo4j.Id = UniqueSequenceGenerator.GenerateUniqueLongIdToNeo4j();

        await _graphClient.Cypher
            .Create("(u:User {id: $id, email: $email, password: $password, role: $role})")
            .WithParams(new
            {
                id = userNeo4j.Id,
                email = userNeo4j.Email,
                password = userNeo4j.Password,
                role = userNeo4j.Role
            })
            .ExecuteWithoutResultsAsync();

    }


    public async Task<List<User>> GetAll()
    {
        var query = await _graphClient.Cypher
            .Match("(a:User)")  
            .Return(a => a.As<Neo4jUser>())  
            .ResultsAsync;

        var users = query.ToList();
        Console.Write("data" + query.ToJson());

        return _mapper.Map<List<User>>(users);;  
    }

    public async Task<User?> GetByEmail(string email)
        {
            var user = await _graphClient.Cypher
            .Match("(a:User)")
            .Where((Neo4jUser a) => a.Email == email)
            .Return(a => a.As<Neo4jUser>())  
            .ResultsAsync;

            var singleNeo4jUser = user.SingleOrDefault();

            return singleNeo4jUser == null ? null : _mapper.Map<User>(singleNeo4jUser);
        }

    public async Task<User?> GetUserById(long id)
        {
            var user = await _graphClient.Cypher
            .Match("(a:User)")
            .Where((Neo4jUser a) => a.Id == id)
            .Return(a => a.As<Neo4jUser>())  
            .ResultsAsync;

            var singleNeo4jUser = user.SingleOrDefault();

            return singleNeo4jUser == null ? null : _mapper.Map<User>(singleNeo4jUser);
        }
}
