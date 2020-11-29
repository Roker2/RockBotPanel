using System.ComponentModel.DataAnnotations;

namespace RockBotPanel.ViewModels.Account
{
    public class DeleteUserViewModel
    {
        [Required]
        [DataType("Password")]
        public string ValidationCode { get; set; }
    }
}
