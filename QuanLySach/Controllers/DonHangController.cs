using Microsoft.AspNetCore.Mvc;

namespace QuanLySach.Controllers
{
	public class DonHangController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
