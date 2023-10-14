using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using QuanLySach.Helpper;
using QuanLySach.Models;

namespace QuanLySach.Areas.Admin.Controllers
{
    [Area("Admin")]
	[Authorize]
	public class TinDangsController : Controller
    {
        public INotyfService _notyfService { get; }
        private readonly BooksContext _context;

        public TinDangsController(BooksContext context, INotyfService notyfService)
        {
            _notyfService = notyfService;
            _context = context;
        }

        // GET: Admin/TinDangs
        public IActionResult Index(int page = 1, int CatID = 0)
        {
            var PageNumBer = page;
            var PageSize = 100;
            List<TinDang> lsNew = new List<TinDang>();
            if (CatID != 0)
            {
                lsNew = _context.TinDangs.AsNoTracking().Where(x => x.CatId == CatID).Include(x => x.Cat).OrderByDescending(x => x.PostId).ToList();
            }
            else
            {
                lsNew = _context.TinDangs.AsNoTracking().Include(x => x.Cat).OrderByDescending(x => x.PostId).ToList();
            }
            PagedList<TinDang> models = new PagedList<TinDang>(lsNew.AsQueryable(), PageNumBer, PageSize);
            ViewBag.CatID = CatID;
            ViewBag.CurrentPage = PageNumBer;
            ViewData["danhmuc"] = new SelectList(_context.Categories, "CatId", "CatName");
            return View(models);

        }
        public IActionResult Filtter(int CatID = 0)
        {
            var url = $"/Admin/TinDangs?CatID={CatID}";
            if (CatID == 0)
            {
                url = $"/Admin/TinDangs";
            }
            return Json(new { status = "success", redirectUrl = url });
        }

        // GET: Admin/TinDangs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TinDangs == null)
            {
                return NotFound();
            }

            var tinDang = await _context.TinDangs.Include(x=>x.Cat)
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (tinDang == null)
            {
                return NotFound();
            }

            return View(tinDang);
        }

        // GET: Admin/TinDangs/Create
        public IActionResult Create()
        {
            ViewData["danhmuc"] = new SelectList(_context.Categories, "CatId", "CatName");
            return View();
        }

        // POST: Admin/TinDangs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PostId,Title,Scontents,Contents,Thumb,Published,Alias,CreatedDate,Author,AccountId,Tags,CatId,IsHot,IsNewfeed,MetaKey,MetaDesc,Views")] TinDang tinDang, Microsoft.AspNetCore.Http.IFormFile fThumb)
        {
            if (ModelState.IsValid)
            {
                tinDang.Title = Utilities.ToTitleCase(tinDang.Title!);
                    string extension = Path.GetExtension(fThumb.FileName);
                    string image = Utilities.SEOUrl(tinDang.Title) + extension ;
                    tinDang.Thumb = await Utilities.UploadFile(fThumb, @"tinDangs", image.ToLower());
                tinDang.CreatedDate = DateTime.Now;
                _notyfService.Success("thêm thành công");
                _context.Add(tinDang);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["danhmuc"] = new SelectList(_context.Categories, "CatId", "CatName");
            return View(tinDang);
        }

        // GET: Admin/TinDangs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.TinDangs == null)
            {
                return NotFound();
            }

            var tinDang = await _context.TinDangs.FindAsync(id);
            if (tinDang == null)
            {
                return NotFound();
            }
            ViewData["danhmuc"] = new SelectList(_context.Categories, "CatId", "CatName",tinDang.PostId);
            return View(tinDang);
        }

        // POST: Admin/TinDangs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PostId,Title,Scontents,Contents,Thumb,Published,Alias,CreatedDate,Author,AccountId,Tags,CatId,IsHot,IsNewfeed,MetaKey,MetaDesc,Views")] TinDang tinDang, Microsoft.AspNetCore.Http.IFormFile fThumb)
        {
            if (id != tinDang.PostId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    tinDang.Title = Utilities.ToTitleCase(tinDang.Title!);
                    if (fThumb != null)
                    {

                        string extension = Path.GetExtension(fThumb.FileName);
                        string image = Utilities.SEOUrl(tinDang.Title) + extension ;
                        tinDang.Thumb = await Utilities.UploadFile(fThumb, @"tinDangs", image.ToLower());

                    }
                    if (string.IsNullOrEmpty(tinDang.Thumb)) tinDang.Thumb = "default.jpg";
                    tinDang.CreatedDate = DateTime.Now;
                    _notyfService.Success("sửa thành công");
                    _context.Update(tinDang);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TinDangExists(tinDang.PostId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["danhmuc"] = new SelectList(_context.Categories, "CatId", "CatName", tinDang.PostId);
            return View(tinDang);
        }

        // GET: Admin/TinDangs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.TinDangs == null)
            {
                return NotFound();
            }

            var tinDang = await _context.TinDangs
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (tinDang == null)
            {
                return NotFound();
            }

            return View(tinDang);
        }

        // POST: Admin/TinDangs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.TinDangs == null)
            {
                return Problem("Entity set 'BooksContext.TinDangs'  is null.");
            }
            var tinDang = await _context.TinDangs.FindAsync(id);
            if (tinDang != null)
            {
                _context.TinDangs.Remove(tinDang);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TinDangExists(int id)
        {
          return (_context.TinDangs?.Any(e => e.PostId == id)).GetValueOrDefault();
        }
    }
}
