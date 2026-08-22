using System.ComponentModel.DataAnnotations;

namespace HMS_Temp_Humdity_Auth.DTOs
{
	public class LoginRequest
	{
		[Required(ErrorMessage = "Tên đăng nhập không được để trống")]
		public string Phone { get; set; } = string.Empty;
		[Required(ErrorMessage = "Mật khẩu không được để trống")]
		public string Password { get; set; } = string.Empty;
		public bool RememberLogin { get; set; }
	}
}
