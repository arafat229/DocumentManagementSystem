namespace DocumentManagementSystem.Helpers
{
    public class QueryObject
    {
        public int? Id { get; set; } = null;
        public decimal? Price { get; set; }
        public string? Tilte { get; set; } = null;
        public string? Content { get; set; } = null;
        public string? Author { get; set; } = null;
        public DateTime? CreationDate { get; set; } = null;
    }
}
