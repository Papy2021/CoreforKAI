using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Core.Modeles
{
    public class TeamMember
    {
      
        public int? Id { get; set; }

        [AllowNull]
        [Required]
        [MaxLength(70, ErrorMessage ="Out Of Range")]
        public string Name { get; set; }

        [Required]
        [AllowNull]
        public Position? Position { get; set; }

        [Display(Name="Current Country")]
        public string? Country { get; set; }

        public string? Nationality { get; set; }


        [Phone]
        [Required]
        public string? Phone { set; get; }

        //[Required]
        [AllowNull]
        public string? PhotoPath { get; set; }

        [MaxLength(180, ErrorMessage = "Out Of Range")]
        public string? Summary { get; set; }

    }
}
