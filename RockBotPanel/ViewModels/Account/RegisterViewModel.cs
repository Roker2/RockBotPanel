using System.ComponentModel.DataAnnotations;

namespace RockBotPanel.ViewModels.Account
{
    public class RegisterViewModel
    {
        public RegisterViewModel()
        {

        }
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public int TelegramId { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password",
            ErrorMessage = "Password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
