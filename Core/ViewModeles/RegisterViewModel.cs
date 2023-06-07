using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Core.ViewModeles
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Remote(action: "IsEmailUsed", controller: "Account")]
        public string? Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? Password { get; set; }


        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage ="Confirm Password Doesn't Match")]
        [Display(Name ="Confirm Password")]
        public string? ConfirmPassword { get; set; }
    }
}
