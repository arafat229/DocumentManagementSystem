using DocumentManagementSystem.Model;



namespace DocumentManagementSystem.Model;

public class MyFile
    {
        public long Id { get; set; }
        public string file { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Categories { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public User user { get; set; } = null!;
        public long userId { get; set; }

             
    }


