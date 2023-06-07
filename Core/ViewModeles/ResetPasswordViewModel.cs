using System.ComponentModel.DataAnnotations;

namespace Core.ViewModeles
{
    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        public string? Token { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

  
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage ="Password and confirm password must match")]
        [Display(Name = "Confirm Password")]
        public string? ConfirmPassword { get; set; }
    }
}
