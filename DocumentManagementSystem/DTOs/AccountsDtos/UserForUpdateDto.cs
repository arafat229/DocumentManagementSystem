using System.Globalization;

namespace DocumentManagementSystem.DTOs.AccountsDtos
{
    public record UserForUpdateDto
    {
        public string Id { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Email { get; set; } = null!;
        public bool IsActive { get; set; }
        public string Role { get; set; } = null!;
        public List<string> Permissions { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }




    }
}
