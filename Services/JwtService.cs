using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HMS_Temp_Humdity_Auth.Config;
using HMS_Temp_Humdity_Auth.Models.HMS_Temp_Humdity_Auth.Models;
using HMS_Temp_Humdity_Auth.Services.Interface;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace HMS_Temp_Humdity_Auth.Services
{
	public class JwtService : IJwtService
	{
		private readonly JwtSettings _settings;

		public JwtService(IOptions<JwtSettings> options)
		{
			_settings = options.Value;
		}

		public string GenerateRefreshToken(UserAuthModel user)
		{
			var expiresAt = DateTime.UtcNow.AddDays(_settings.RefreshTokenExpiresInDays);

			var claims = new List<Claim>
			{
				new(JwtRegisteredClaimNames.Sub, user.UserId),
				new(ClaimTypes.NameIdentifier, user.UserId),
				new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
				new("token_type", "refresh")
			};

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey));
			var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				issuer: _settings.Issuer,
				audience: _settings.Audience,
				claims: claims,
				notBefore: DateTime.UtcNow,
				expires: expiresAt,
				signingCredentials: credentials
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}

		public TokenResult GenerateToken(UserAuthModel user)
		{
			var expiresAt = DateTime.UtcNow.AddMinutes(_settings.ExpiresInMinutes);

			var claims = new List<Claim>
			{
				new(JwtRegisteredClaimNames.Sub,user.UserId),
				new(ClaimTypes.NameIdentifier,user.UserId),
				new(ClaimTypes.Name,user.Username),
				new(ClaimTypes.Role,user.Role.ToString()),
				new("companyId",user.CompanyId ?? string.Empty),
				new("companyPermissions",((long)user.CompanyPermissions).ToString()),
				new(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
			};

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey));

			var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				issuer: _settings.Issuer,
				audience: _settings.Audience,
				claims: claims,
				expires: expiresAt,
				signingCredentials: credentials
			);

			return new TokenResult
			{
				AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
				ExpiresAt = expiresAt
			};
		}

		public ClaimsPrincipal ValidateRefreshToken(string refreshToken)
		{
			var tokenHandler = new JwtSecurityTokenHandler();

			var parameters = new TokenValidationParameters
			{
				ValidateIssuer = true,
				ValidIssuer = _settings.Issuer,
				ValidateAudience = true,
				ValidAudience = _settings.Audience,
				ValidateLifetime = true,
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey)),
				ClockSkew = TimeSpan.Zero
			};

			var principal = tokenHandler.ValidateToken(refreshToken, parameters, out SecurityToken validatedToken);
			var tokenType = principal.FindFirst("token_type")?.Value;

			if (tokenType != "refresh") throw new UnauthorizedAccessException("Token không phải Refresh Token");

			return principal;
		}
	}
}
