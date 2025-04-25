namespace DocumentManagementSystem.DTOs.DocumentsDtos
{
    public record FileForCreationDto
    {
        public string Name { get; set; } = null!;
        public IFormFile File { get; set; }= null!;
        public string? Description { get; set; } 


    }
}
