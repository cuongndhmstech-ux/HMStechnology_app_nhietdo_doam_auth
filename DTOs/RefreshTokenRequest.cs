using System.Text.Json.Serialization;

namespace HMS_Temp_Humdity_Auth.DTOs
{
	public class RefreshTokenRequest
	{
		[JsonPropertyName("refreshToken")]
		public required string RefreshToken { get; set; }
	}
}
