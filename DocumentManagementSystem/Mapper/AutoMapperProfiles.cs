using AutoMapper;
using DocumentManagementSystem.DTOs.Account;
using DocumentManagementSystem.DTOs.AccountsDtos;
using DocumentManagementSystem.DTOs.DocumentsDtos;
using DocumentManagementSystem.Model;

namespace DocumentManagementSystem.Mapper
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User,UserForRegisterDto>().ReverseMap();
            CreateMap<User, UserDetailsDto>().ReverseMap();
            CreateMap<User, UserForListDto>().ReverseMap();
            CreateMap<User, UserForUpdateDto>();
            CreateMap<FileForCreationDto,MyFile>();
            CreateMap<MyFile, FileForSearchDto>().ReverseMap();
            CreateMap<MyFile, FileForUpdateDto>().ReverseMap();

        }
    }
}
