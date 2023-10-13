using System.ComponentModel.DataAnnotations;

namespace QuanLySach.Areas.Admin.Models
{
    public class LoginAdmin
    {
        [Key]
        [MaxLength(100)]
        [Required(ErrorMessage =("Vui lòng nhập Email"))]
        [Display(Name ="Địa chỉ Email")]
        [EmailAddress(ErrorMessage ="Đây không phải Email")]
        public string ? UserName { get; set; }

        [Display(Name ="Mật Khẩu")]
        [Required(ErrorMessage ="Vui Lòng nhập mật khẩu")]
        [MinLength(5,ErrorMessage =" Mật khẩu tối thiểu 5 ký tự")]

        public string? Password { get; set; }


    }
}
