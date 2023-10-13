using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLySach.Models;
using QuanLySach.ModelsView;
using System.Linq.Expressions;

namespace QuanLySach.Controllers
{
	public class DonHangsController : Controller
	{
		private readonly BooksContext _context;
		public INotyfService _notyfService { get; }
		public DonHangsController(BooksContext context, INotyfService notyfService)
		{
			_context = context;
			_notyfService = notyfService;
		}
		[HttpPost]
		public async Task<IActionResult> Details(int ? id)
		{
			if(id==null) return NotFound();
			try{
				var customerID = HttpContext.Session.GetString("CustomerId");
				if (string.IsNullOrEmpty(customerID)) return RedirectToAction("Login", "Customer");
				var customer = _context.Customers.AsNoTracking().SingleOrDefault(x => x.CustomerId == Convert.ToInt32(customerID));
				if (customer == null) return NotFound();
				var donhang = await _context.Orders.AsNoTracking()
					.Include(x => x.TransactStatus)
					.FirstOrDefaultAsync(x => x.OrderId == id && Convert.ToInt32(customerID) == x.CustomerId);
				if (donhang == null) return NotFound();

				var chitietdonhang = _context.OrderDetails
					.Include(x => x.Product)
					.Where(x => x.OrderId == id)
					.OrderByDescending(x => x.CreateDate).
					ToList();
				DonHang dh = new DonHang();
				dh.XemDonHang = donhang;
				dh.ChiTietDonHang = chitietdonhang;
				return PartialView("Details", dh);




			}
			catch
			{
				return NotFound();

			}

		}
	}
}
