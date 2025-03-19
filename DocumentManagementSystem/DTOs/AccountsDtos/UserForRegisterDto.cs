using System.ComponentModel.DataAnnotations;

namespace DocumentManagementSystem.DTOs.Account
{
    public record UserForRegisterDto
    {
        [Phone]
        public string PhoneNumber { get; set; } = null!;
        [Required]
        public string LastName { get; set; } = null!;
        [Required]
        public string FirstName { get; set; } = null!;
        [Required]
        public string Username { get; set; } = null!;
        public string Role { get; set; } = null!;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;


    }
}
