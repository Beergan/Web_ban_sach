
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Security.Permissions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using QuanLySach.Models;

namespace QuanLySach.Controllers
{
	public class ProductController : Controller
	{
		private readonly BooksContext _context;
		public ProductController(BooksContext context)
		{
			_context = context;
		}
		//Get:Index
		[Route("products.html", Name = "Product")]
		public IActionResult Index(int page = 1, int  CatID = 0 )
		{
			try
			{
				var PageSize = 100;
				var PageNumber = page;
				
				var lsCat = _context.Categories.AsTracking().OrderByDescending(x => x.CatId).ToList();
				var ls = _context.Products.AsNoTracking().Include(x => x.Cat).OrderByDescending(x => x.ProductId);
				PagedList<Product> lsProduct = new PagedList<Product>(ls.AsQueryable(), PageNumber, PageSize);
				ViewBag.lsCat = lsCat;
				ViewData["danhmuc"] = new SelectList(_context.Categories, "CatId", "CatName");
				ViewBag.CurrentPage = PageNumber;
				return View(lsProduct);
				
			}
			catch
			{
				return RedirectToAction("Index","Home");
			}
		
		}
		[Route("/product/{ProductName}-{id}.html", Name = "ProductDetails")]
		public IActionResult Details(int id) {
			try
			{
				var product = _context.Products.AsNoTracking().Include(x => x.Cat).SingleOrDefault(x => x.ProductId == id);
				if (product == null)
				{
					return RedirectToAction("Index");
				}
				var lsProduct = _context.Products.AsNoTracking().Include(x => x.Cat)

					.Where(x => x.ProductId != id && x.UnitsInStock > 0)
					.OrderByDescending(x => x.CatId).OrderByDescending(x => x.BestSellers).ToList();
				ViewBag.lsProductlienquan = lsProduct;
				//var lsCat = _context.Categories.Join(_context.Products,x=>x.CatId,b=>b.Cat.CatId,(x,c)
				//	=>new
				//	{
				//		CatID = x.CatId
				//	}
				//	).Where(a=>a.)
				var lsCat = _context.Categories.AsTracking().OrderByDescending(x => x.CatId).ToList();
				var lsProductLQ = _context.Products.AsNoTracking().Where(x => x.ProductId != id && x.Active == true).Take(3).OrderByDescending(x => x.CatId).ToList();
				ViewBag.lsProductLQ = lsProductLQ;
				ViewBag.lsCatDS = lsCat;
				return View(product);
			}
			catch {
				return RedirectToAction("Index", "Home");
			}
		}
		[Route("/danhmuc/{CatID}.html", Name = "ProductDanhMuc")]
		public IActionResult List(int CatID, int page=1)
		{
			//try {
				var PageSize = 100;
				var PageNumber = page;
				var danhmuc = _context.Categories.AsNoTracking().
					SingleOrDefault(x => x.CatId == CatID);
				var ls = _context.Products.AsNoTracking().Include(x => x.Cat).Where(x => x.CatId == danhmuc!.CatId).
					OrderByDescending(x => x.DateCreated);

				PagedList<Product> lsProducts = new PagedList<Product>(ls, PageNumber, PageSize);
				var lsCat = _context.Categories.AsNoTracking().OrderByDescending(x => x.CatId);
				ViewBag.CurrentPage = PageNumber;
				ViewBag.CurrentCat = danhmuc;
				return View(lsProducts);
			//}
			//catch {
			//	return RedirectToAction("Index", "Home");
			//}

			
		}

	}
}
