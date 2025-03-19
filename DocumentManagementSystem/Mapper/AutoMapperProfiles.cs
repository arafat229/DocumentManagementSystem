using AutoMapper;
using DocumentManagementSystem.DTOs.Account;
using DocumentManagementSystem.Model;

namespace DocumentManagementSystem.Mapper
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User,UserForRegisterDto>().ReverseMap();
        }
    }
}
