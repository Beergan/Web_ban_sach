using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace QuanLySach.Areas.Admin.Controllers
{
    
    [Area("Admin")]
    [Route("admin.html", Name = "AdminIndex")]
    [Authorize]
	public class HomeController : Controller
	{// GET: /<controller>/
		public IActionResult Index()
        {
            return View();
        }
    }
}
