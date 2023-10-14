using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLySach.Models;
using PagedList.Core;
using AspNetCoreHero.ToastNotification.Abstractions;
using AspNetCoreHero.ToastNotification.Notyf;
using System.Drawing.Printing;
using Microsoft.AspNetCore.Authorization;

namespace QuanLySach.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class AuthorsController : Controller
    {
        private readonly BooksContext _context;
		public INotyfService _notyfService { get; }

		public AuthorsController(BooksContext context, INotyfService notyfService)
        { 
            _notyfService = notyfService;
            _context = context;
        }

        // GET: Admin/Authors
        [Route("tacgia.html",Name = "Authors")]
        public IActionResult Index( int page =1 , int CatID = 0    )
        {
            var pageNumber = page;
            var pageSize = 20;
            List<Author> lsAuthor = new List<Author>();
            if (CatID != 0)
            {
                lsAuthor = _context.Authors
                .AsNoTracking()
                .Where(x => x.CatId == CatID)
                .Include(x => x.Cat)
                .OrderBy(x => x.AuthorId).ToList();
            }
            else
            {
                lsAuthor = _context.Authors
                .AsNoTracking()
                .Include(x => x.Cat)
                .OrderBy(x => x.AuthorId).ToList();
            }



            PagedList<Author> models = new PagedList<Author>(lsAuthor.AsQueryable(), pageNumber, pageSize);
            ViewBag.CurrentCateID = CatID;
            ViewBag.CurrentPage = pageNumber;
            ViewData["danhmuc"] = new SelectList(_context.Categories, "CatId", "CatName");


            return View(models);
		}

        // GET: Admin/Authors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Authors == null)
            {
                return NotFound();
            }

            var author = await _context.Authors
                .Include(a => a.Cat)
                .FirstOrDefaultAsync(m => m.AuthorId == id);
            if (author == null)
            {
                return NotFound();
            }

            return View(author);
        }

        // GET: Admin/Authors/Create
        public IActionResult Create()
        {
            ViewData["CatId"] = new SelectList(_context.Categories, "CatId", "CatName");
            return View();
        }

        // POST: Admin/Authors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AuthorId,FullName,Title,Date,Active,CreateDate,CatId")] Author author)
        {
            if (ModelState.IsValid)
            {
                _context.Add(author);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CatId"] = new SelectList(_context.Categories, "CatId", "CatName", author.CatId);
            return View(author);
        }

        // GET: Admin/Authors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Authors == null)
            {
                return NotFound();
            }

            var author = await _context.Authors.FindAsync(id);
            if (author == null)
            {
                return NotFound();
            }
            ViewData["CatId"] = new SelectList(_context.Categories, "CatId", "CatName", author.CatId);
            return View(author);
        }

        // POST: Admin/Authors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AuthorId,FullName,Title,Date,Active,CreateDate,CatId")] Author author)
        {
            if (id != author.AuthorId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(author);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AuthorExists(author.AuthorId))
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
            ViewData["CatId"] = new SelectList(_context.Categories, "CatId", "CatName", author.CatId);
            return View(author);
        }

        // GET: Admin/Authors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Authors == null)
            {
                return NotFound();
            }

            var author = await _context.Authors
                .Include(a => a.Cat)
                .FirstOrDefaultAsync(m => m.AuthorId == id);
            if (author == null)
            {
                return NotFound();
            }

            return View(author);
        }

        // POST: Admin/Authors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Authors == null)
            {
                return Problem("Entity set 'BooksContext.Authors'  is null.");
            }
            var author = await _context.Authors.FindAsync(id);
            if (author != null)
            {
                _context.Authors.Remove(author);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AuthorExists(int id)
        {
          return (_context.Authors?.Any(e => e.AuthorId == id)).GetValueOrDefault();
        }
    }
}
