using DocumentManagementSystem.DTOs.DocumentsDtos;
using DocumentManagementSystem.Model;
using System.ComponentModel.DataAnnotations;

namespace DocumentManagementSystem.DTOs.Account
{
    public record UserForRegisterDto
    {
        [Phone]
        public string PhoneNumber { get; set; } = null!;
        [Required]
        public string FullName { get; set; } = null!;
        [Required]
        public string Username { get; set; } = null!;
        public string Role { get; set; } = null!;

        [EmailAddress]
        public string Email { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;

       



    }
}
