using DocumentManagementSystem.DTOs.DocumentsDtos;
using DocumentManagementSystem.Model;
using System.ComponentModel.DataAnnotations;

namespace DocumentManagementSystem.DTOs.AccountsDtos
{
    public record  UserForListDto
    {
        
        public string PhoneNumber { get; set; } = null!;

        public string Role { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;




    }
    
}
