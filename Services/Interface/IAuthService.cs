using HMS_Temp_Humdity_Auth.DTOs;

namespace HMS_Temp_Humdity_Auth.Services.Interface
{
	public interface IAuthService
	{
		Task<LoginResponse> LoginAsync(LoginRequest request);
		Task<LoginResponse> RefreshTokenAsync(RefreshTokenRequest request);
	}
}
