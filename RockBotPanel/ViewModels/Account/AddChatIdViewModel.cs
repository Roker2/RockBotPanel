using System.ComponentModel.DataAnnotations;

namespace RockBotPanel.ViewModels.Account
{
    public class AddChatIdViewModel
    {

        [Required]
        public long ChatId { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Code { get; set; }
    }
}
