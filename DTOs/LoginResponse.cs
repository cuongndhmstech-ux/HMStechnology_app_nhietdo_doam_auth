using HMS_Temp_Humdity_Auth.Models.HMS_Temp_Humdity_Auth.Models;

namespace HMS_Temp_Humdity_Auth.DTOs
{
	public class LoginResponse
	{
		public required string AccessToken { get; set; }

		public string? RefreshToken { get; set; }

		//public string? FullName { get; set; }

		//public string? Phone { get; set; }

		public Role Role { get; set; }

		public DateTime AccessTokenExpiresAt { get; set; }

		public DateTime? RefreshTokenExpiresAt { get; set; }
	}
}
