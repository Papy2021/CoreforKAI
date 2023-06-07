using System.ComponentModel.DataAnnotations;

namespace Core.ViewModeles
{
    public class CreateRoleViewModel
    {
        [Required]
        [Display(Name = "Role Name")]
        public string? RoleName { get; set; }
    }
}
