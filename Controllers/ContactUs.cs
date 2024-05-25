using Microsoft.AspNetCore.Mvc;

namespace KhumaloCraft.Controllers
{
    public class ContactUs : Controller
    {
        public IActionResult ContactUsIndex()
        {
            return View();
        }
    }
}
