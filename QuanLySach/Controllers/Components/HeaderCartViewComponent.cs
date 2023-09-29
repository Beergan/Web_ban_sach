using Microsoft.AspNetCore.Mvc;
using QuanLySach.ModelsView;
using QuanLySach.Extension;
using Microsoft.AspNetCore.Http;



namespace QuanLySach.Controllers.Components
{
	public class HeaderCartViewComponent : ViewComponent
	{
		public IViewComponentResult Invoke()
		{
			List<CartItemS> cart = HttpContext.Session.Get<List<CartItemS>>("GioHang");
			return View(cart);
		}
	}
}
