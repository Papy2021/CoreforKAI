using Core.Modeles;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Core.ViewModeles
{
    public class CreateMemberViewModel
    {
        [AllowNull]
        [Required]
        [MaxLength(70, ErrorMessage = "Out Of Range")]
        public string Name { get; set; }

        [Required]
        [AllowNull]
        public Position? Position { get; set; }

        [Display(Name = "Current Country")]
        public string? Country { get; set; }

        public string? Nationality { get; set; }


        [Phone]
        [Required]
        public string? Phone { set; get; }


        [MaxLength(180, ErrorMessage = "Out Of Range")]
        public string? Summary { get; set; }


        [AllowNull]
        public IFormFile Photo { get; set; }
    }
}
