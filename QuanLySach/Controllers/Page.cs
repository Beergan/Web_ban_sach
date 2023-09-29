using Microsoft.AspNetCore.Mvc;

namespace QuanLySach.Controllers
{
	public class Page : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
