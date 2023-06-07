using System.ComponentModel.DataAnnotations;

namespace Core.ViewModeles
{
    public class AddPasswordViewModel
    {
        //this ViewModel is used for the external login

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string? NewPassword { get; set; }

        [Required]
        [DataType (DataType.Password)]
        [Display(Name = "Confirm New Password")]
        [Compare("NewPassword", ErrorMessage ="The new password and confirm password do not match")]
        public string? ConfirmPassword { get; set; }
        }
    }
