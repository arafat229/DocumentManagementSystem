using DocumentManagementSystem.Model;

namespace DocumentManagementSystem.DTOs.DocumentsDtos
{
    public record FileForReturnDto
    {
        public long Id { get; set; }
        public string File { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
