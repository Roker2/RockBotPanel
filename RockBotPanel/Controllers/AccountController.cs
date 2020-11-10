using Microsoft.AspNetCore.Mvc;

namespace RockBotPanel.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
    }
}
