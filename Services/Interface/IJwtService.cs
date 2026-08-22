using System.Security.Claims;
using HMS_Temp_Humdity_Auth.Models.HMS_Temp_Humdity_Auth.Models;

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
		public required string AccessToken { get; set; }

		public DateTime ExpiresAt { get; set; }
	}
}
