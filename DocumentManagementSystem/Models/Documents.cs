namespace DocumentManagementSystem.Model
{
    public class Documents
    {

        public int Id { get; set; }
        public int Document { get; set; }
        public string Tilte { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }




    }
}
