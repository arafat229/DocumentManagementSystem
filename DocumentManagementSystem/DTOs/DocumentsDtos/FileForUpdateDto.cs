using DocumentManagementSystem.Model;

namespace DocumentManagementSystem.DTOs.DocumentsDtos
{
    public record FileForUpdateDto
    {

      
        public string? Name { get; set; } 
        public IFormFile? File { get; set; } 
        public string? Description { get; set; } 
        

    }
}
