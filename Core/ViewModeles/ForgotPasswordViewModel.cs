using System.ComponentModel.DataAnnotations;

namespace Core.ViewModeles
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string? Email { set; get; }
    }
}
