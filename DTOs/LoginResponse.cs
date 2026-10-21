using System.Text.Json.Serialization;
using HMS_Temp_Humdity_Auth.Models;

namespace HMS_Temp_Humdity_Auth.DTOs
{
	public class LoginResponse
	{
		[JsonPropertyName("accessToken")]
		public required string AccessToken { get; set; }

		[JsonPropertyName("refreshToken")]
		public string? RefreshToken { get; set; }

		[JsonPropertyName("role")]
		public Role Role { get; set; }

		[JsonPropertyName("accessTokenExpiresAt")]
		public DateTime AccessTokenExpiresAt { get; set; }

		[JsonPropertyName("refreshTokenExpiresAt")]
		public DateTime? RefreshTokenExpiresAt { get; set; }
	}
}
