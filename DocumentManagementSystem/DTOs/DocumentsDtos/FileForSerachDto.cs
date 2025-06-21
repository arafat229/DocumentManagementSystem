using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace DocumentManagementSystem.DTOs.DocumentsDtos
{
    public record FileForSerachDto
    {

        public string? SearchingTerm { get; set; } 

    }
}
