using AutoMapper;
using backend.Dtos;

namespace backend.Config
{
    public class MappingProfile: Profile
    {

        public MappingProfile() {
            CreateMap<User, UserResponse>();
            CreateMap<User, JwtRequest>();
        }
    }
}
