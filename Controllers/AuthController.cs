using HMS_Temp_Humdity_Auth.DTOs;
using HMS_Temp_Humdity_Auth.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMS_Temp_Humdity_Auth.Controllers
{
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly IAuthService _authService;

		public AuthController(IAuthService authService)
		{
			_authService = authService;
		}

		[AllowAnonymous]
		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginRequest request)
		{
			try
			{
				var result = await _authService.LoginAsync(request);
				return Ok(new
				{
					success = true,
					message = "Đăng nhập thành công",
					data = result
				});
			}
			catch (UnauthorizedAccessException ex)
			{
				return Unauthorized(new
				{
					success = false,
					message = ex.Message,
					data = (object?)null
				});
			}
		}
		[HttpPost("refresh-token")]
		public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
		{
			var result = await _authService.RefreshTokenAsync(request);

			return StatusCode(200, new ApiResponse<object>
			{
				Success = true,
				Message = "Làm mới token thành công",
				Data = result
			});
		}
	}
}
