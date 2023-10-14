using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuanLySach.Models;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using Microsoft.Identity.Client;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace QuanLySach.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class AdminLoginController : Controller
    {
        private readonly BooksContext _context;
        public INotyfService _notyfService { get;  }
        public AdminLoginController(
            BooksContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }
        // GET: /<controller>/
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        [Route("loginadmin.html",Name ="LoginAmin")]
        public IActionResult AdminLogin(string? returnUrl = null)
        {
            var adminId = HttpContext.Session.GetString("AccountId");
            if (adminId != null)
            {
                return RedirectToAction("Index", "Home", new { Areas = "Admin" });
            }
            ViewBag.returnUrl = returnUrl;
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("loginadmin.html",Name = "LoginAmin")]
        public async Task<IActionResult> AdminLogin(Models.LoginAdmin model, string? returnUrl = null)
        {
            try {
                if(ModelState.IsValid)
                {
                    Account? tk = _context.Accounts
                        .Include(x => x.Role)
                        .SingleOrDefault(x => x.Email!.ToLower() == model.UserName!.ToLower().Trim());
                    if (tk == null)
                    {
						ViewBag.Error = "Thông tin đăng nhập chưa chính xác";
						return View(model);
					}
                    var pass = (model.Password!.Trim());

					if (pass != tk.Password!.Trim())
                    {

						ViewBag.Error = " Thông tin đăng nhập chưa chính xác";
                        return View(model);
                    }
                    tk.LastLogin = DateTime.Now;
                    _context.Update(tk);
                    await _context.SaveChangesAsync();

                    var tkID = HttpContext.Session.GetString("AccountId");
                    var Claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, tk.Email!),
                        new Claim("AccountId", tk.AccountId.ToString()),
                        new Claim(ClaimTypes.Email, tk.Email!),
                        new Claim("RoleId",tk.RoleId!.ToString()!),
                        new Claim(ClaimTypes.Name, tk.Role!.RoleName!)


                    };
                    var Identity = new ClaimsIdentity(Claims, "User Identity");
                    Identity.AddClaim(new Claim(ClaimTypes.Role, "Admin"));
                    var Principal = new ClaimsPrincipal(new[] { Identity });

					await HttpContext.SignInAsync(Principal);
					return RedirectToAction("Index", "Home", new {Area = "Admin"});
                }
            }
            catch
            {
                return RedirectToAction("AdminLogin", "AdminLogin", new { Area = "Admin" });
            }
            return RedirectToAction("AdminLogin", "AdminLogin", new { Area = "Admin" });

        }
		[Route("logout.html", Name = "Logout")]
        public IActionResult Adminlogout()
        {
            try
            {
                HttpContext.SignOutAsync();
                HttpContext.Session.Remove("AccountId");
				return RedirectToAction("AdminLogin", "AdminLogin", new { Area = "Admin" });
			}
            catch {
				return RedirectToAction("AdminLogin", "AdminLogin", new { Area = "Admin" });
			}
        }

	}
}
