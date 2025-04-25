 namespace DocumentManagementSystem.DTOs.DocumentsDtos
{
    public record FileForReturnDto
    {
        public string File { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
