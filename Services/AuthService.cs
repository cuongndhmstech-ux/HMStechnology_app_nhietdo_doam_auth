using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HMS_Temp_Humdity_Auth.Config;
using HMS_Temp_Humdity_Auth.Database.Interface;
using HMS_Temp_Humdity_Auth.DTOs;
using HMS_Temp_Humdity_Auth.Models;
using HMS_Temp_Humdity_Auth.Services.Interface;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace HMS_Temp_Humdity_Auth.Services
{
	public class AuthService : IAuthService
	{
		private readonly IDAOUserAuth _userAuthDao;
		private readonly JwtSettings _jwtSettings;
		private readonly IJwtService _jwtService;

		public AuthService(IDAOUserAuth userAuthDao, IOptions<JwtSettings> jwtOptions, IJwtService jwtService)
		{
			_userAuthDao = userAuthDao;
			_jwtSettings = jwtOptions.Value;
			_jwtService = jwtService;
			ValidateJwtSettings();
		}

		public async Task<LoginResponse> LoginAsync(LoginRequest request)
		{
			if (string.IsNullOrWhiteSpace(request.Phone))
			{
				throw new ArgumentException("Số điện thoại không được để trống");
			}

			if (string.IsNullOrWhiteSpace(request.Password))
			{
				throw new ArgumentException("Mật khẩu không được để trống");
			}

			var phone = request.Phone.Trim();
			var user = await _userAuthDao.GetByUsernameAsync(phone);
			if (user == null)
			{
				throw new UnauthorizedAccessException("Tên đăng nhập hoặc mật khẩu không đúng");
			}

			var isValid = BCrypt.Net.BCrypt.Verify(
				request.Password,
				user.Password
			);

			if (!isValid)
			{
				throw new UnauthorizedAccessException("Tên đăng nhập hoặc mật khẩu không đúng");
			}

			var accessTokenExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiresInMinutes);

			var accessToken = GenerateAccessToken(user, accessTokenExpiresAt);

			string? refreshToken = null;
			DateTime? refreshTokenExpiresAt = null;

			if (request.RememberLogin)
			{
				refreshTokenExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiresInDays);
				refreshToken = GenerateRefreshToken(user, refreshTokenExpiresAt.Value);
			}

			return new LoginResponse
			{
				AccessToken = accessToken,
				RefreshToken = refreshToken,
				AccessTokenExpiresAt = accessTokenExpiresAt,
				RefreshTokenExpiresAt = refreshTokenExpiresAt,
				Role = user.Role
			};
		}

		private string GenerateAccessToken(
			UserAuthModel user,
			DateTime expiresAt)
		{
			var signingCredentials = CreateSigningCredentials();

			var claims = new List<Claim>
			{
				new(JwtRegisteredClaimNames.Sub, user.UserId),
				new(ClaimTypes.NameIdentifier, user.UserId),
				new(ClaimTypes.Name, user.UserName),
				new(ClaimTypes.Role, user.Role.ToString()),
				new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
				new("token_type", "access"),
				new("companyPermissions", ((long)user.CompanyPermissions).ToString())
			};

			if (!string.IsNullOrWhiteSpace(user.CompanyId))
			{
				claims.Add(new Claim("companyId", user.CompanyId));
			}

			var token = new JwtSecurityToken(
				issuer: _jwtSettings.Issuer,
				audience: _jwtSettings.Audience,
				claims: claims,
				notBefore: DateTime.UtcNow,
				expires: expiresAt,
				signingCredentials: signingCredentials
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}

		private string GenerateRefreshToken(UserAuthModel user, DateTime expiresAt)
		{
			var signingCredentials = CreateSigningCredentials();
			var claims = new List<Claim>
			{
				new(JwtRegisteredClaimNames.Sub, user.UserId),
				new(ClaimTypes.NameIdentifier, user.UserId),
				new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
				new("token_type", "refresh")
			};

			var token = new JwtSecurityToken(
				issuer: _jwtSettings.Issuer,
				audience: _jwtSettings.Audience,
				claims: claims,
				notBefore: DateTime.UtcNow,
				expires: expiresAt,
				signingCredentials: signingCredentials
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}

		private SigningCredentials CreateSigningCredentials()
		{
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));

			return new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
		}

		private void ValidateJwtSettings()
		{
			if (string.IsNullOrWhiteSpace(_jwtSettings.SecretKey))
			{
				throw new InvalidOperationException("Jwt:SecretKey chưa được cấu hình");
			}

			if (_jwtSettings.SecretKey.Length < 32)
			{
				throw new InvalidOperationException("Jwt:SecretKey phải có ít nhất 32 ký tự");
			}

			if (string.IsNullOrWhiteSpace(_jwtSettings.Issuer))
			{
				throw new InvalidOperationException("Jwt:Issuer chưa được cấu hình");
			}

			if (string.IsNullOrWhiteSpace(_jwtSettings.Audience))
			{
				throw new InvalidOperationException("Jwt:Audience chưa được cấu hình");
			}

			if (_jwtSettings.ExpiresInMinutes <= 0)
			{
				throw new InvalidOperationException("Jwt:ExpiresInMinutes phải lớn hơn 0");
			}

			if (_jwtSettings.RefreshTokenExpiresInDays <= 0)
			{
				throw new InvalidOperationException("Jwt:RefreshTokenExpiresInDays phải lớn hơn 0");
			}
		}

		public async Task<LoginResponse> RefreshTokenAsync(
			RefreshTokenRequest request)
		{
			var principal = _jwtService.ValidateRefreshToken(
				request.RefreshToken);

			var userId = principal.FindFirst(
				ClaimTypes.NameIdentifier)?.Value;

			if (string.IsNullOrWhiteSpace(userId))
				throw new UnauthorizedAccessException(
					"Refresh Token không hợp lệ");

			var user = await _userAuthDao.GetByUserIdAsync(userId);

			if (user == null)
				throw new UnauthorizedAccessException(
					"Người dùng không tồn tại");

			var accessTokenExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiresInMinutes);
			var refreshTokenExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiresInDays);

			var accessToken = GenerateAccessToken(user, accessTokenExpiresAt);
			string? refreshToken = GenerateRefreshToken(user, refreshTokenExpiresAt);

			return new LoginResponse
			{
				AccessToken = accessToken,
				RefreshToken = refreshToken,
				AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(
					_jwtSettings.ExpiresInMinutes),
				RefreshTokenExpiresAt = DateTime.UtcNow.AddDays(
					_jwtSettings.RefreshTokenExpiresInDays),
				Role = user.Role
			};
		}
	}
}