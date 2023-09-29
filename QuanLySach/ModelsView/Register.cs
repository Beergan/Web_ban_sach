using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System;

namespace QuanLySach.ModelsView
{
    public class Register
    {
        [Key]
        public int CustomerId { get; set; }

        [Display(Name = "Họ và Tên")]
        [Required(ErrorMessage = "Vui lòng nhập Họ Tên")]
        public string ? FullName { get; set; }

        [MaxLength(150)]
        [Required(ErrorMessage = "Vui lòng nhập Email")]
		
		[DataType(DataType.EmailAddress)]
		[Remote(action: "CheckEmail", controller: "Customer")]
        public string ? Email { get; set; }

        [MaxLength(11)]
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Display(Name = "Điện thoại")]
		[DataType(DataType.PhoneNumber)]
		[Remote(action: "CheckPhone", controller: "Customer")]
		[MinLength(10, ErrorMessage = "Bạn cần đặt mật khẩu tối thiểu 10 số")]
        public string ? Phone { get; set; }

        [Display(Name = "Mật khẩu")]
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [MinLength(5, ErrorMessage = "Bạn cần đặt mật khẩu tối thiểu 5 ký tự")]
        public string? Password { get; set; }

        [MinLength(5, ErrorMessage = "Bạn cần đặt mật khẩu tối thiểu 5 ký tự")]
        [Display(Name = "Nhập lại mật khẩu")]
        [Compare("Password", ErrorMessage = "Nhập lại mật khẩu không đúng")]
        public string ? ConfirmPassword { get; set; }
        [Required(ErrorMessage ="Vui lòng xác minh")]
        public bool checkbox {  get; set; }
    }
}
