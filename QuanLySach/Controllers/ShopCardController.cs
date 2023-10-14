using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using QuanLySach.Extension;
using QuanLySach.Models;
using QuanLySach.ModelsView;


namespace QuanLySach.Controllers
{
    public class ShopCardController : Controller
    {
        private readonly BooksContext _context;
        public INotyfService _notyfService { get; }
        public ShopCardController(BooksContext context, INotyfService notyfService)
        {
            _notyfService = notyfService;
            _context = context;
        }
        public List<CartItemS> GioHang
        {
            get
            {
                var gh = HttpContext.Session.Get<List<CartItemS>>("GioHang");
                if (gh == default(List<CartItemS>))
                {
                    gh = new List<CartItemS>();
                }
                return gh;
            }
        }

        [HttpPost]
        [Route("api/cart/add")]
        public IActionResult AddToCart(int productID, int? amount)
        {
            List<CartItemS> cart = GioHang;

            try
            {
                //Them san pham vao gio hang
                CartItemS? item = cart.SingleOrDefault(p => p.product!.ProductId == productID);
                if (item != null) // da co => cap nhat so luong
                {
                    item.amount = item.amount + amount!.Value;
                    //luu lai session
                    HttpContext.Session.Set<List<CartItemS>>("GioHang", cart);
                }
                else
                {
                    Product? hh = _context.Products.SingleOrDefault(p => p.ProductId == productID);
                    item = new CartItemS
                    {
                        amount = amount.HasValue ? amount.Value : 1,
                        product = hh!
                    };
                    cart.Add(item);//Them vao gio
                }

                //Luu lai Session
                HttpContext.Session.Set<List<CartItemS>>("GioHang", cart);
                _notyfService.Success("Thêm sản phẩm thành công");
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }
        [HttpPost]
        [Route("api/cart/update")]
        public IActionResult UpdateCart(int productID, int? amount)
        {
            //Lay gio hang ra de xu ly
            var cart = HttpContext.Session.Get<List<CartItemS>>("GioHang");
            try
            {
                if (cart != null)
                {
                    CartItemS? item = cart.SingleOrDefault(p => p.product!.ProductId == productID);
                    if (item != null && amount.HasValue) // da co -> cap nhat so luong
                    {
                        item.amount = amount.Value;
                    }
                    //Luu lai session
                    HttpContext.Session.Set<List<CartItemS>>("GioHang", cart);
                }
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }

        [HttpPost]
        [Route("api/cart/remove")]
        public ActionResult Remove(int productID)
        {

            try
            {
                List<CartItemS> gioHang = GioHang;
                CartItemS? item = gioHang.SingleOrDefault(p => p.product!.ProductId == productID);
                if (item != null)
                {
                    gioHang.Remove(item);
                }
                //luu lai session
                HttpContext.Session.Set<List<CartItemS>>("GioHang", gioHang);
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
        }
        [Route("cart.html", Name = "Cart")]
        public IActionResult Index()
        {
            return View(GioHang);
        }
    }
}

