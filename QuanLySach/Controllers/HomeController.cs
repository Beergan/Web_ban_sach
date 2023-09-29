using Microsoft.AspNetCore.Mvc;
using QuanLySach.Models;
using System.Diagnostics;

namespace QuanLySach.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Blog()
        {
            return View();
        }
        public IActionResult Cart()
        {
            return View();
        }
        public IActionResult Contact() 
        {
            return View(); 
        }
        public IActionResult Checkout()
        {
            return View();
        }
        public IActionResult Shop()
        {
            return View();
        }
        public IActionResult Char()
        {
            return View();
        }


        [Route("/Login.html" , Name ="Login") ]
        public IActionResult Login()
        {
            return View();
        }
		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}