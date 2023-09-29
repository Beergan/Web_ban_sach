using Microsoft.AspNetCore.Mvc;

namespace QuanLySach.Controllers
{
	public class AjaxController : Controller
	{
		public IActionResult NumberCart()
		{
			return ViewComponent("NumberCart");
		}
		public IActionResult HeadrCart()
		{
			return ViewComponent("HeadrCart");
		}
	}
}
