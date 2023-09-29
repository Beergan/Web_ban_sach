using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;

using QuanLySach.Models;

namespace QuanLySach.Controllers
{
    public class BolgController : Controller
    {
        private readonly BooksContext _context;
        public BolgController(BooksContext context) {
            _context = context;
        }
        //GET:Index
        [Route("bolgs.html",Name="Blog")]
        public IActionResult Index(int  page = 1 )
        {
            var PageNumBer = page;
            var PageSize = 100;
            var lstingDangs = _context.TinDangs.AsNoTracking().OrderByDescending(x => x.PostId);
            PagedList<TinDang> model = new PagedList<TinDang>(lstingDangs,PageNumBer,PageSize);
            ViewBag.CurrentPage = PageNumBer;
            return View(model);
        }
        [Route("/tin-tuc/{Alias}-{id}.html",Name ="TinDetails")]
        public IActionResult Details(int id )
        {
            var tingdang = _context.TinDangs.AsNoTracking().Include(x=>x.Cat).SingleOrDefault(x => x.PostId == id);
            if (tingdang == null)
            {
                return RedirectToAction("Index");
            }
			var lsLienQuan = _context.TinDangs.AsNoTracking().Include(x => x.Cat).Where(x => x.Published == true && x.PostId!=id).Take(3).OrderByDescending(x => x.CatId).ToList();
			ViewBag.BaiVietLQ = lsLienQuan;


			return View(tingdang);
        }
    }
}
