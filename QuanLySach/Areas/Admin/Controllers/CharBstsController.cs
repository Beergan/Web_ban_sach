using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using AspNetCoreHero.ToastNotification.Notyf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using QuanLySach.Helpper;
using QuanLySach.Models;

namespace QuanLySach.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CharBstsController : Controller
    {
        private readonly BooksContext _context;
        public INotyfService _notyfService { get; }

        public CharBstsController(BooksContext context, INotyfService notyfService)
        {
            _notyfService= notyfService;
            _context = context;
        }

        // GET: Admin/CharBsts
        public IActionResult Index(int page = 1, int ProductID = 0)
        {
            var PageNumber = page;
            var PageSize = 20;
            List<CharBst> lsChar = new();
            if (ProductID != 0)
            {
                lsChar = _context.CharBsts
                .AsNoTracking()
                .Where(x => x.ProductId == ProductID )
                .Include(x => x.Product)
                .OrderBy(x => x.CharId).ToList();
            }
            else
            {
                lsChar = _context.CharBsts
                .AsNoTracking()
                .Include(x => x.Product)
                .OrderBy(x => x.CharId).ToList();
            }

            PagedList<CharBst> models = new PagedList<CharBst>(lsChar.AsQueryable(), PageNumber, PageSize);
            ViewBag.CurrentProduct = ProductID;
            ViewBag.CurrentPage = PageNumber;
            
            ViewData["danhmuc"] = new SelectList(_context.Products, "ProductId", "ProductName");
            ViewData["danhmuc1"] = new SelectList(_context.Categories, "CatId", "CatName");

            return View(models);
        }
        public IActionResult Filtter(int ProductID = 0)
        {
            var url = $"/Admin/CharBsts?ProductId={ProductID}";
            if (ProductID == 0)
            {
                url = $"/Admin/CharBsts";
            }
            return Json(new { status = "success", redirectUrl = url });
        }

        // GET: Admin/CharBsts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.CharBsts == null)
            {
                return NotFound();
            }

            var charBst = await _context.CharBsts
                .Include(c => c.Product)
                .FirstOrDefaultAsync(m => m.CharId == id);
            if (charBst == null)
            {
                return NotFound();
            }

            return View(charBst);
        }

        // GET: Admin/CharBsts/Create
        public IActionResult Create()
        {
            ViewData["danhmuc"] = new SelectList(_context.Products, "ProductId", "ProductName");
            return View();
        }

        // POST: Admin/CharBsts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CharId,CharName,Contents,Thumb,Published,Title,Alias,CreatedDate,Ordering,ProductId,Price,UnitsInStock,Description,Active,HomeFlag,BestSellers,RateId")] CharBst charBst, Microsoft.AspNetCore.Http.IFormFile fThumb)
        {
            if (ModelState.IsValid)
            {
                charBst.CharName = Utilities.ToTitleCase(charBst.CharName!);
                if (fThumb != null)
                {
                    string ex = Path.GetExtension(fThumb.FileName);
                    String img = Utilities.SEOUrl(charBst.CharName) + ex;
                    charBst.Thumb = await Utilities.UploadFile(fThumb, @"CharBTS", img.ToLower());

                }
                if(string.IsNullOrEmpty(charBst.Thumb)) { charBst.Thumb = "default.jpg"; }
                charBst.Alias = Utilities.SEOUrl(charBst.CharName);
                _context.Add(charBst);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["danhmuc"] = new SelectList(_context.Products, "ProductId", "ProductName", charBst.ProductId);
            return View(charBst);
        }

        // GET: Admin/CharBsts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.CharBsts == null)
            {
                return NotFound();
            }

            var charBst = await _context.CharBsts.FindAsync(id);
            if (charBst == null)
            {
                return NotFound();
            }
            ViewData["danhmuc"] = new SelectList(_context.Products, "ProductId", "ProductName", charBst.ProductId);
            return View(charBst);
        }

        // POST: Admin/CharBsts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CharId,CharName,Contents,Thumb,Published,Title,Alias,CreatedDate,Ordering,ProductId,Description,Active,HomeFlag,BestSellers,RateId")] CharBst charBst, Microsoft.AspNetCore.Http.IFormFile fThumb)
        {
            if (id != charBst.CharId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    charBst.CharName = Utilities.ToTitleCase(charBst.CharName!);
                    if (fThumb != null)
                    {
                        string ex = Path.GetExtension(fThumb.FileName);
                        String img = Utilities.SEOUrl(charBst.CharName) + ex;
                        charBst.Thumb = await Utilities.UploadFile(fThumb, @"CharBTS", img.ToLower());

                    }
                    if (string.IsNullOrEmpty(charBst.Thumb)) { charBst.Thumb = "default.jpg"; }
                    charBst.Alias = Utilities.SEOUrl(charBst.CharName);
                    _notyfService.Success("Cập nhật thành công");
                    _context.Update(charBst);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CharBstExists(charBst.CharId))
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
            ViewData["danhmuc"] = new SelectList(_context.Products, "ProductId", "ProductName", charBst.ProductId);
            return View(charBst);
        }

        // GET: Admin/CharBsts/Delete/5 
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.CharBsts == null)
            {
                return NotFound();
            }

            var charBst = await _context.CharBsts
                .Include(c => c.Product)
                .FirstOrDefaultAsync(m => m.CharId == id);
            if (charBst == null)
            {
                return NotFound();
            }

            return View(charBst);
        }

        // POST: Admin/CharBsts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.CharBsts == null)
            {
                return Problem("Entity set 'BooksContext.CharBsts'  is null.");
            }
            var charBst = await _context.CharBsts.FindAsync(id);
            if (charBst != null)
            {
                _context.CharBsts.Remove(charBst);
            }
            _notyfService.Success("Xóa Thành công");
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CharBstExists(int id)
        {
          return (_context.CharBsts?.Any(e => e.CharId == id)).GetValueOrDefault();
        }
    }
}
