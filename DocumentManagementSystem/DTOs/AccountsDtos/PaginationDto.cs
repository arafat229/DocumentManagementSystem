namespace DocumentManagementSystem.DTOs.AccountsDtos
{
    public record PaginationDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        public string? OrderBy { get;set; }
        public string? Query { get; set; }

    }
}
