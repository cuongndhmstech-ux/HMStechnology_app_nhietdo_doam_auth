using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HMS_Temp_Humdity_Auth.DTOs
{
	public class LoginRequest
	{
		[Required(ErrorMessage = "Tên đăng nhập không được để trống")]
		[JsonPropertyName("phone")]
		public string Phone { get; set; } = string.Empty;

		[Required(ErrorMessage = "Mật khẩu không được để trống")]
		[JsonPropertyName("password")]
		public string Password { get; set; } = string.Empty;

		[JsonPropertyName("rememberLogin")]
		public bool RememberLogin { get; set; }
	}
}
