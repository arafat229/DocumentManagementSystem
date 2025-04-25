namespace DocumentManagementSystem.DTOs.Account
{
    public record UserDetailsDto
    {
        public string Id { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Fullname { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Email { get; set; } = null!;
        public bool IsActive { get; set; }
        public string Role { get; set; } = null!;
        public List<string> Permission { get; set; } = null!;
        public DateTime IsCreatedAt { get; set; } = DateTime.Now;
     //   public DateTime IsUpdatedAt { get; set; }

    }
}
