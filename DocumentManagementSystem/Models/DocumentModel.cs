using DocumentManagementSystem.Model;



namespace DocumentManagementSystem.Model;

public class MyFile
    {
        public long Id { get; set; }
        public string File { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public User user { get; set; } = null!;
        public long? userId { get; set; }

             
    }


