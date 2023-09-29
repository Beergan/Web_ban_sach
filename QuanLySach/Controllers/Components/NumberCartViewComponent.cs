using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuanLySach.Extension;
using QuanLySach.ModelsView;

namespace QuanLySach.Controllers.Components
{
	public class NumberCartViewComponent : ViewComponent
	{
		public IViewComponentResult Invoke()
		{
			var cart = HttpContext.Session.Get<List<CartItemS>>("GioHang");
			return View(cart);
		}
	}
}
