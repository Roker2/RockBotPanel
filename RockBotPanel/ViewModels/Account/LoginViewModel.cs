using System.ComponentModel.DataAnnotations;

namespace RockBotPanel.ViewModels.Account
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [DataType("Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }

        [DataType("Password")]
        [Display(Name = "Validation code")]
        public string ValidationCode { get; set; }
    }
}
