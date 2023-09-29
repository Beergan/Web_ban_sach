using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLySach.Models;


// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace QuanLySach.Areas.Admin.Controllerst
{
    [Area("Admin")]
    public class SearchController : Controller
    {
        private readonly BooksContext _context;

        public SearchController(BooksContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult FindProduct(string keyword)
        {
            List<Product> ls = new List<Product>();
            if (string.IsNullOrEmpty(keyword) || keyword.Length < 1)
            {
                return PartialView("ListProductsSearchPartial", null);
            }
            ls = _context.Products.AsNoTracking()
                                  .Include(a => a.Cat)
                                  .Where(x => x.ProductName.Contains(keyword))
                                  .OrderByDescending(x => x.ProductName)
                                  .Take(10)
                                  .ToList();
            if (ls == null)
            {
                return PartialView("ListProductsSearchPartial", null);
            }
            else
            {
                return PartialView("ListProductsSearchPartial", ls);
            }
        }
        [HttpPost]
        public IActionResult FindChar(string keyword)
        {

            List<CharBst> ls = new List<CharBst>();
            if (string.IsNullOrEmpty(keyword) || keyword.Length < 1)
            {
                return PartialView("ListCharBstSearchPartial", null);
            }
            ls = _context.CharBsts.AsNoTracking()
                                  .Include(a => a.Product)
                                  .Where(x => x.CharName.Contains(keyword))
                                  .OrderByDescending(x => x.CharName)
                                  .Take(10)
                                  .ToList();
            if (ls == null)
            {
                return PartialView("ListCharBstSearchPartial", null);
            }
            else
            {
                return PartialView("ListCharBstSearchPartial", ls);
            }
        }
        public IActionResult FindPages(string keyword)
        {

            List<Page> ls = new List<Page>();
            if (string.IsNullOrEmpty(keyword) || keyword.Length < 1)
            {
                return PartialView("ListPageSearchPartial", null);
            }
            ls = _context.Pages.AsNoTracking()
                                  .Include(a => a.Cat)
                                  .Where(x => x.PageName.Contains(keyword))
                                  .OrderByDescending(x => x.CreatedDate)
                                  .Take(10)
                                  .ToList();
            if (ls == null)
            {
                return PartialView("ListPageSearchPartial", null);
            }
            else
            {
                return PartialView("ListPageSearchPartial", ls);
            }
        }
        public IActionResult FindTinDangs(string keyword)
        {

            List<TinDang> ls = new List<TinDang>();
            if (string.IsNullOrEmpty(keyword) || keyword.Length < 1)
            {
                return PartialView("ListTinDangsSearchPartial", null);
            }
            ls = _context.TinDangs.AsNoTracking()
                                  .Include(a => a.Cat)
                                  .Where(x => x.Title.Contains(keyword))
                                  .OrderByDescending(x => x.CreatedDate)
                                  .Take(10)
                                  .ToList();
            if (ls == null)
            {
                return PartialView("ListTinDangsSearchPartial", null);
            }
            else
            {
                return PartialView("ListTinDangsSearchPartial", ls);
            }
        }
    }
}
