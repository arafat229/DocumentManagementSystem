using DocumentManagementSystem.Model;
using System.ComponentModel.DataAnnotations.Schema;



namespace DocumentManagementSystem.Model;

public class MyFile
    {
        public long Id { get; set; } 
        public string File { get; set; } = null!;
        public string Extractedtext { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        [NotMapped]
        public User user { get; set; } = null!;
        public long userId { get; set; } 

             
    }


        