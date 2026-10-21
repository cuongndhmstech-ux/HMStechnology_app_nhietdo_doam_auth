using System.Security.Claims;
using System.Text.Json.Serialization;
using HMS_Temp_Humdity_Auth.Models;

namespace HMS_Temp_Humdity_Auth.Services.Interface
{
	public interface IJwtService
	{
		TokenResult GenerateToken(UserAuthModel user);
		string GenerateRefreshToken(UserAuthModel user);

		ClaimsPrincipal ValidateRefreshToken(string refreshToken);
	}

	public class TokenResult
	{
		[JsonPropertyName("accessToken")]
		public required string AccessToken { get; set; }

		[JsonPropertyName("expiresAt")]
		public DateTime ExpiresAt { get; set; }
	}
}
