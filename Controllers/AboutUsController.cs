using Microsoft.AspNetCore.Mvc;

namespace KhumaloCraft.Controllers
{
    public class AboutUsController : Controller
    {
        public IActionResult AboutUsIndex()
        {
            return View();
        }
    }
}
