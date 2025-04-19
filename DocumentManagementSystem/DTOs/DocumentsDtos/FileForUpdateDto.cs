using DocumentManagementSystem.Model;

namespace DocumentManagementSystem.DTOs.DocumentsDtos
{
    public record FileForUpdateDto
    {

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string file { get; set; } = null!;
        public string Categories { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime CreatedAt { get; set; }

    }
}
