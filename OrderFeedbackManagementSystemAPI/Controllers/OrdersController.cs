using Microsoft.AspNetCore.Mvc;

namespace OrderFeedbackManagementSystemAPI.Controllers
{
    public class OrdersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
