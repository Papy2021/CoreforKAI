using System.ComponentModel.DataAnnotations;

namespace Core.ViewModeles
{
    public class EditUserViewModel
    {
        public EditUserViewModel()
        {
            Claims = new List<string>();
            Roles = new List<string>();
        }


        public string? Id { get; set; }

        [Display(Name ="User Name")]
        [Required]
        public string? UserName { get; set; }
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        public List<string>? Claims { get; set; }
        public List<string>? Roles { get; set; }


    }
}
