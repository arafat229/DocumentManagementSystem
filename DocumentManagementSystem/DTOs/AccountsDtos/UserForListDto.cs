using System.ComponentModel.DataAnnotations;

namespace DocumentManagementSystem.DTOs.AccountsDtos
{
    public record  UserForListDto
    {
        [Phone]
        public string PhoneNumber { get; set; } = null!;
        public string  Role { get; set; } = null!;
        public string  FirstName { get; set; } = null!;
        public string  LastName { get; set; } = null!;
        public string  UserName { get; set; } = null!;
        public string  Password { get; set; } = null!;
        [EmailAddress]
        public string email { get; set; } = null!;
        
    }
}
