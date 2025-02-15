using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DocumentManagementSystem.DTOs.DocManagementSystemDto
{
    public class DocManagementSystemDto
    {

        public int Id { get; set; }

        [Required]
        [Column(TypeName =("Decimal(16,2)"))]
        public decimal Price { get; set; }

        [Required]
        [MaxLength(20,ErrorMessage ="Title cannot be over 10 charater")]
        public string Tilte { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        [Required]
        [MaxLength(20, ErrorMessage = "Title cannot be over 10 charater")]
        public string? Author { get; set; }

        [Required]
        public DateTime CreationDate { get; set; } = DateTime.Now;
    }
}
