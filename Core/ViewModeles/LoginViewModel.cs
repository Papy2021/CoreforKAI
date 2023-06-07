using Microsoft.AspNetCore.Authentication;
using System.ComponentModel.DataAnnotations;

namespace Core.ViewModeles
{
    public class LoginViewModel
    {
        public LoginViewModel()
        {
            ExternalLogins = new List<AuthenticationScheme>();   
        }

        [Required]
        [EmailAddress]
        [Display(Name ="Email Address")]
        public string? Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Display(Name ="Remember me")]
        public bool RememberMe { get; set; }
        public string? ReturnUrl { get; set; }
        public IList<AuthenticationScheme>? ExternalLogins { get; set; }
    }
}
