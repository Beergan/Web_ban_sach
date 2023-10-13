using QuanLySach.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QuanLySach.ModelsView
{
	public class DonHang
	{
		
		public Order ? XemDonHang {  get; set; }
		public List<OrderDetail> ? ChiTietDonHang { get; set; }
	}
}
