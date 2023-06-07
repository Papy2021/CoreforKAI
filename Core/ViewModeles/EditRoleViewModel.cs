using System.ComponentModel.DataAnnotations;

namespace Core.ViewModeles
{
    public class EditRoleViewModel
    {
        public EditRoleViewModel()
        {
            Users = new List<string>();  
        }

        public string? Id { get; set; }
        [Required]
        [Display(Name = "Role Name")]
        public string? RoleName  { get; set; }

        public List<string>? Users { get; set; }
    }
}
