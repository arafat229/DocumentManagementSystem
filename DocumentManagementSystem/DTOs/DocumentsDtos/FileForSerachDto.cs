using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace DocumentManagementSystem.DTOs.DocumentsDtos
{
    public record FileForSerachDto
    {

       
        public long Id { get; set; }

        public string SearchingTerm { get; set; } = null!;

        [DisplayFormat(DataFormatString ="{0:MM/dd/yyyy}",ApplyFormatInEditMode =true)]
         public DateTime? CreatedAt { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? UpdatedAt { get; set; }
       
    }
}
