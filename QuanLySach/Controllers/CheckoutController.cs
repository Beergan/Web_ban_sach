﻿using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLySach.Extension;
using QuanLySach.Helpper;
using QuanLySach.Models;
using QuanLySach.ModelsView;

namespace QuanLySach.Controllers
{
	public class CheckoutController : Controller
	{
		private readonly BooksContext _context;
		public INotyfService _notyfService { get; }
		public CheckoutController(BooksContext context, INotyfService notyfService)
		{
			_context = context;
			_notyfService = notyfService;
		}
		public List<CartItemS> GioHang
		{
			get
			{
				var gh = HttpContext.Session.Get<List<CartItemS>>("GioHang");
				if (gh == default(List<CartItemS>))
				{
					gh = new List<CartItemS>();
				}
				return gh;
			}
		}
		[Route("checkout.html", Name = "Checkout")]
		public IActionResult Index(string? returnUrl = null)
		{
			//Lay gio hang ra de xu ly
			List<CartItemS> cart = HttpContext.Session.Get<List<CartItemS>>("GioHang");
			var taikhoanID = HttpContext.Session.GetString("CustomerId");
			MuaHangVM model = new MuaHangVM();
			if (taikhoanID != null)
			{
				var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x => x.CustomerId == Convert.ToInt32(taikhoanID));
				model.CustomerId = khachhang!.CustomerId;
				model.FullName = khachhang.FullName;
				model.Email = khachhang.Email;
				model.Phone = khachhang.Phone;
				model.Address = khachhang.Address;
			}
			ViewBag.GioHang = cart;
			return View(model);
		}

		[HttpPost]
		[Route("checkout.html", Name = "Checkout")]
		public IActionResult Index(MuaHangVM muaHang)
		{
			//Lay ra gio hang de xu ly
			var cart = HttpContext.Session.Get<List<CartItemS>>("GioHang");
			var taikhoanID = HttpContext.Session.GetString("CustomerId");
			MuaHangVM model = new MuaHangVM();
			if (taikhoanID != null)
			{
				var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x => x.CustomerId == Convert.ToInt32(taikhoanID));
				model.CustomerId = khachhang!.CustomerId;
				model.FullName = khachhang.FullName;
				model.Email = khachhang.Email;
				model.Phone = khachhang.Phone;
				khachhang.Address = muaHang.Address;


			
				

				_context.Update(khachhang);
				_context.SaveChanges();
			}
			try
			{
				if (ModelState.IsValid)
				{
					//Khoi tao don hang
					Order donhang = new Order();
					donhang.CustomerId = model.CustomerId;
					donhang.Address = model.Address;
					donhang.OrderDate = DateTime.Now;
					donhang.TransactStatusId = 1;//Don hang moi
					donhang.Deleted = false;
					donhang.Paid = false;
					donhang.Note = Utilities.StripHTML(model.Note!);
					donhang.TotalMoney = Convert.ToInt32(cart.Sum(x => x.TotalMoney));
					_context.Add(donhang);
					_context.SaveChanges();
					//tao danh sach don hang
					foreach (var item in cart)
					{
						OrderDetail orderDetail = new OrderDetail();
						orderDetail.OrderId = donhang.OrderId;
						orderDetail.ProductId = item.product!.ProductId;
						orderDetail.Amount = item.amount;
						orderDetail.TotalMoney = donhang.TotalMoney;
						orderDetail.Price = item.product.Price;
						orderDetail.CreateDate = DateTime.Now;
						_context.Add(orderDetail);
					}
                    
					//clear gio hang
					HttpContext.Session.Remove("GioHang");
					//Xuat thong bao
					_notyfService.Success("Đơn hàng đặt thành công");
					//cap nhat thong tin khach hang
					return RedirectToAction("Success");


				}
			}
			catch
			{
				ViewBag.GioHang = cart;
				return View(model);
			}
			ViewBag.GioHang = cart;
			return View(model);
		}
		[Route("dat-hang-thanh-cong.html", Name = "Success")]
		public IActionResult Success()
		{
			try
			{
				var taikhoanID = HttpContext.Session.GetString("CustomerId");
				if (string.IsNullOrEmpty(taikhoanID))
				{
					return RedirectToAction("Login", "Accounts", new { returnUrl = "/dat-hang-thanh-cong.html" });
				}
				var khachhang = _context.Customers.AsNoTracking().SingleOrDefault(x => x.CustomerId == Convert.ToInt32(taikhoanID));
				var donhang = _context.Orders
					.Where(x => x.CustomerId == Convert.ToInt32(taikhoanID))
					.OrderByDescending(x => x.OrderDate)
					.FirstOrDefault();
				MuaHangSuccessVM successVM = new MuaHangSuccessVM();
				successVM.FullName = khachhang!.FullName;
				successVM.DonHangID = donhang!.OrderId;
				successVM.Phone = khachhang.Phone;
				successVM.Address = khachhang.Address;
				
				
                return View(successVM);
			}
			catch
			{
				return View();
			}
		}
		
	}
}
