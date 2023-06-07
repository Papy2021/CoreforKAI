using System.ComponentModel.DataAnnotations;

namespace Core.ViewModeles
{
    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        public string? CurrentPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string? NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirme Password")]
        [Compare("NewPassword", ErrorMessage ="New password and confirm password doesn't match")]
        public string? ConfirmPassword { get; set; }
    }
}
