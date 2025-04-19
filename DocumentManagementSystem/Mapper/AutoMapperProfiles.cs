using AutoMapper;
using DocumentManagementSystem.DTOs.Account;
using DocumentManagementSystem.DTOs.DocumentsDtos;
using DocumentManagementSystem.Model;

namespace DocumentManagementSystem.Mapper
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User,UserForRegisterDto>().ReverseMap();
            CreateMap<FileForCreationDto, MyFile>().ReverseMap();
            CreateMap<MyFile, FileForReturnDto>().ReverseMap();
            CreateMap<MyFile, FileForUpdateDto>().ReverseMap();

        }
    }
}
