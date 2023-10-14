using QuanLySach.Models;

namespace QuanLySach.ModelsView
{
    public class CartItemS
    {

        public Product? product { get; set; }
        public int amount { get; set; }
        public double TotalMoney => amount * product!.Price!.Value;
    }
}
