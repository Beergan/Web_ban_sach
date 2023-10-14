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
	public class PagesController : Controller
    {
        public INotyfService _notyfService { get; }
        private readonly BooksContext _context;

        public PagesController(BooksContext context, INotyfService notyfService)
        {
            _notyfService = notyfService;
            _context = context;
        }

        // GET: Admin/Pages
        public IActionResult Index(int page = 1 ,int CatID = 0)
        {
            var PageNumber = page;
            var PageSize = 20;
            List < Page > lsPage = new List<Page> ();
            if (CatID != 0)
            {
                lsPage = _context.Pages.AsNoTracking().Where(x=>x.CatId==CatID).Include(x=>x.Cat)
                    .OrderByDescending(x=>x.PageId).ToList();

            }
            else
            {
                lsPage = _context.Pages.AsNoTracking().Include(x => x.Cat)
                    .OrderByDescending(x => x.PageId).ToList();

            }
            PagedList<Page> models = new PagedList<Page>(lsPage.AsQueryable(), PageNumber, PageSize);
            ViewBag.CatID = CatID;
            ViewBag.CurrentPage = PageNumber;
            ViewData["danhmuc"] = new SelectList(_context.Categories, "CatId", "CatName");

            return View(models);

        }
        public IActionResult Filtter(int CatID = 0)
        {
            var url = $"/Admin/Pages?CatID={CatID}";
            if (CatID == 0)
            {
                url = $"/Admin/Pages";
            }
            return Json(new { status = "success", redirectUrl = url });
        }

        // GET: Admin/Pages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Pages == null)
            {
                return NotFound();
            }

            var page = await _context.Pages
                .FirstOrDefaultAsync(m => m.PageId == id);
            if (page == null)
            {
                return NotFound();
            }

            return View(page);
        }

        // GET: Admin/Pages/Create
        public IActionResult Create()
        {
            ViewData["danhmuc"] = new SelectList(_context.Categories, "CatId", "CatName");
            return View();
        }

        // POST: Admin/Pages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PageId,PageName,Contents,Thumb,Published,Title,MetaDesc,MetaKey,Alias,CreatedDate,Ordering,CatId")] Page page, Microsoft.AspNetCore.Http.IFormFile fThumb)
        {
            
            if (ModelState.IsValid)
            {
                    page.PageName = Utilities.ToTitleCase(page.PageName!);
                    page.CreatedDate = DateTime.Now;
                    if (fThumb != null)
                    {
                        string extension = Path.GetExtension(fThumb.FileName);
                    string image = Utilities.SEOUrl(page.PageName) + extension;
                        page.Thumb = await Utilities.UploadFile(fThumb, @"pages",image.ToLower());
                    
                    }
                if (string.IsNullOrEmpty(page.Thumb)) page.Thumb = "default.jpg";
                
                _notyfService.Success("thêm thành công");
                _context.Add(page);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["danhmuc"] = new SelectList(_context.Categories, "CatId", "CatName");
            return View(page);
        }

        // GET: Admin/Pages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Pages == null)
            {
                return NotFound();
            }

            var page = await _context.Pages.FindAsync(id);
            if (page == null)
            {
                return NotFound();
            }
            ViewData["danhmuc"] = new SelectList(_context.Categories, "CatId", "CatName", page.CatId);
            return View(page);
        }

        // POST: Admin/Pages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PageId,PageName,Contents,Thumb,Published,Title,MetaDesc,MetaKey,Alias,CreatedDate,Ordering,CatId")] Page page, Microsoft.AspNetCore.Http.IFormFile fThumb)
        {
            if (id != page.PageId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                 
                    page.PageName = Utilities.ToTitleCase(page.PageName!);
                    if (fThumb != null)
                    {
                        string extension = Path.GetExtension(fThumb.FileName);
                        string image = Utilities.SEOUrl(page.PageName)  + extension;
                        page.Thumb = await Utilities.UploadFile(fThumb, @"pages", image.ToLower());

                    }
                    if (string.IsNullOrEmpty(page.Thumb))
                    {
                        page.Thumb = page.Thumb;
                    }
                    page.CreatedDate = DateTime.Now;
                    _notyfService.Success("sửa thành công thành công");
                    _context.Update(page);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PageExists(page.PageId))
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
            ViewData["danhmuc"] = new SelectList(_context.Categories, "CatId", "CatName", page.CatId);
            return View(page);
        }

        // GET: Admin/Pages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Pages == null)
            {
                return NotFound();
            }

            var page = await _context.Pages
                .FirstOrDefaultAsync(m => m.PageId == id);
            if (page == null)
            {
                return NotFound();
            }

            return View(page);
        }

        // POST: Admin/Pages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Pages == null)
            {
                return Problem("Entity set 'BooksContext.Pages'  is null.");
            }
            var page = await _context.Pages.FindAsync(id);
            if (page != null)
            {
                _context.Pages.Remove(page);
            }
            _notyfService.Success("Xóa thành công");
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PageExists(int id)
        {
          return (_context.Pages?.Any(e => e.PageId == id)).GetValueOrDefault();
        }
    }
}
