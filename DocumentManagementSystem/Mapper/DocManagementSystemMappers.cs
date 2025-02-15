
using DocumentManagementSystem.DTOs.DocManagementSystemDto;
using DocumentManagementSystem.Model;
using System.ComponentModel.DataAnnotations;

namespace DocumentManagementSystem.Mapper
{
    public static  class DocManagementSystemMappers
    {
        public static DocManagementSystemDto ToDocManagementDto(this DocManagementSystem managemeentSystemDto)
        {
                 return  new DocManagementSystemDto()
            {


              Id = managemeentSystemDto.Id,
             Price = managemeentSystemDto.Price,
             Tilte = managemeentSystemDto.Tilte,
             Content =managemeentSystemDto.Content,
             Author = managemeentSystemDto.Author,
             CreationDate = managemeentSystemDto.CreationDate

            };

        }
    }
}
