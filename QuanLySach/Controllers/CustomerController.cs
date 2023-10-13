using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuanLySach.Extension;
using QuanLySach.Helpper;
using QuanLySach.Models;
using QuanLySach.ModelsView;
using System.Linq;
using PagedList.Core;
using Microsoft.EntityFrameworkCore;

namespace QuanLySach.Controllers
{
    [Authorize]
    public class CustomerController : Controller
    {
        private readonly BooksContext _context;
        public INotyfService _notyfService { get; }
        public CustomerController(BooksContext context, INotyfService notyfService)
        {
            _context = context;
            _notyfService = notyfService;
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult CheckPhone(string Phone)
        {
            try {
                var customers = _context.Customers.AsNoTracking().SingleOrDefault(x => x.Phone!.ToLower() == Phone.ToLower());
                if(customers != null)
                {
                    return Json(data: " Số điện thoại"+Phone + " đã sự dụng ");

                }
                else
                {
                    return Json(data: true);
                }
            }
            catch {
            return Json(data: true);
                    }
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult CheckEmail(string  Email) {
            try {
                var customers = _context.Customers.AsNoTracking().SingleOrDefault(x => x.Email!.ToLower() == Email.ToLower());
                if (customers != null)
                {
                    return Json(data: "Email đã được đăng ký");
                }
                else { return Json(data: true); }
            }
            catch {
                return  Json(data: true);
            }
            }
        [Route("myacount.html",Name ="MyAcount")]
        public IActionResult MyAcount()
        {
            var customerID = HttpContext.Session.GetString("CustomerId");
            if (customerID!=null)
            {
                var customer = _context.Customers.AsNoTracking().SingleOrDefault(x => x.CustomerId == Convert.ToInt32(customerID));
                if (customer != null)
                {
                    var DSdonhang = _context.Orders.AsNoTracking().Include(x=>x.TransactStatus)
                        .Where(x => x.CustomerId == customer.CustomerId).OrderByDescending(x => x.OrderDate).ToList();
                    ViewBag.DonHang = DSdonhang;
                    return View(customer);
                    
                }
                return View(customer);
               
                
            }
            return RedirectToAction("Login");
            
            
        }
        [HttpGet]
        [AllowAnonymous]
        [Route("Register.html",Name = "Register")]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("Register.html", Name = "Register")]
        public async Task<IActionResult> Register(ModelsView.Register Ctm )
        {
            try
            {
                if (ModelState.IsValid)
                {
                   
				    string a = Utilities.GetRandomKey();
                    Customer customer = new Customer
                    {
                        FullName = Ctm.FullName,
                        Phone = Ctm.Phone!.Trim().ToLower(),
                        Email = Ctm.Email!.Trim().ToLower(),
                        Password = (Ctm.Password + a.Trim()).ToMD5(),
                        Active = true,
                        Salt = a,
                        CreateDate = DateTime.Now
                    };
                  
                    
                    
                    try
                    {

                        _context.Add(customer);
                        await _context.SaveChangesAsync();
                        // lưu thông tin khách hàng 
                        HttpContext.Session.SetString("CustomerId", customer.CustomerId.ToString());
                        var CtmID = HttpContext.Session.GetString("CustomerId");
                        // xác minh danh tính
                        var claims = new List<Claim> {
                            new Claim(ClaimTypes.NameIdentifier, customer.CustomerId.ToString()),
                            new Claim("CustomerId", customer.CustomerId.ToString()),
                            new Claim(ClaimTypes.Name, customer.Email),
                        };
                        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "login");
                        // đại điện cho người dùng
                        ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                        await HttpContext.SignInAsync(claimsPrincipal);
                        _notyfService.Success("đăng ký thành công");
                        return RedirectToAction("MyAcount", "Customer");

                    }

                   


                    
                    catch
                    {
                        return RedirectToAction("Register", "Customer");
                    }
                }
                else { return View(Ctm); }
              
            
                

            }
            catch {
                return RedirectToAction("Register", "Customer");
            
            }

        }
        [AllowAnonymous]
        [Route("danhnhap.html", Name = "DangNhap")]
        public IActionResult Login(string ? returnUrl = null)
        {
            var customerID = HttpContext.Session.GetString("CustomerId");
            if (customerID != null) {
                return RedirectToAction("MyAcount", "Customer");
            }
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("danhnhap.html", Name = "DangNhap")]
        public async Task<IActionResult> Login(Login lg,string returnUrl)
        {

            try
            {
                if (lg.UserName!= null) 
                {
                    bool Email = Utilities.IsValidEmail(lg.UserName);
                    if (!Email)  return View(lg); 
                    var  customer = _context.Customers.AsNoTracking().SingleOrDefault(x => x.Email!.Trim() == lg.UserName);
                    if (customer == null)  return RedirectToAction("Register"); 
                    string  pass  = (lg.Password + customer.Salt!.Trim()).ToMD5();
                    if (customer.Password !=pass)
                    {
                        _notyfService.Error("thông tin chưa chính xác");
                        return View(lg);
                    }
                    //kiem tra tk có bị chặn ko ?
                    if (customer.Active == false)
                    {

                        return RedirectToAction("Register", "Customer");

                    }

                    HttpContext.Session.SetString("CustomerId", customer.CustomerId.ToString());
					var taikhoanID = HttpContext.Session.GetString("CustomerId");
                    var claims = new List<Claim>
                    {
						new Claim(ClaimTypes.NameIdentifier, customer.CustomerId.ToString()),
							new Claim("CustomerId", customer.CustomerId.ToString()),
							new Claim(ClaimTypes.Name, customer.Email!),
					};
					
					//var claimsIdentity = new ClaimsIdentity(claims);
					//await HttpContext.SignInAsync(new ClaimsPrincipal(claimsIdentity));
					//await HttpContext.SignInAsync(claimsPrincipal);
					ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "login");
                    ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                    await HttpContext.SignInAsync(claimsPrincipal);
                    if (string.IsNullOrEmpty(returnUrl))
                        {
                        return RedirectToAction("MyAcount", "Customer");
                    }
                    else
                    {
                        return Redirect(returnUrl);
                    }
                }
            }
            catch
            {
                return RedirectToAction("Register", "Customer");

            }
            return View(lg);


        }
        [HttpGet]
        [Route("dangxuat.html", Name = "DangXuat")]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync();
            HttpContext.Session.Remove("CustomerId");
            return RedirectToAction("Index", "Home");
        }


    }
}
