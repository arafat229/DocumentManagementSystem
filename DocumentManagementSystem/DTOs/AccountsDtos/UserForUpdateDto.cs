using System.Globalization;

namespace DocumentManagementSystem.DTOs.AccountsDtos
{
    public record UserForUpdateDto
    {
        public string? PhoneNumber { get; set; }

        public string? FullName { get; set; }

        public string? Username { get; set; }
        public string? Role { get; set; }

        public string? Email { get; set; }



    }
}
