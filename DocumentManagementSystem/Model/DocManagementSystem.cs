using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DocumentManagementSystem.Model
{
    public class DocManagementSystem
    {

        public int Id { get; set; }
        public decimal Price { get; set; }
        public string Tilte { get; set; } = string.Empty;
        public string Content { get; set; }  = string.Empty;
        public string Author { get; set; } = string.Empty;
        public DateTime CreationDate { get; set; }




    }
}
