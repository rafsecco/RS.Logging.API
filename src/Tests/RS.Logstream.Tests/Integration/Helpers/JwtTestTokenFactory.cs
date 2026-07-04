using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace RS.Logstream.Tests.Integration.Helpers;

public static class JwtTestTokenFactory
{
	public static string CreateValidToken(TimeSpan? lifetime = null) =>
		CreateToken(CustomWebApplicationFactory.TestAuthSecret, DateTime.UtcNow.Add(lifetime ?? TimeSpan.FromMinutes(5)));

	public static string CreateExpiredToken() =>
		CreateToken(CustomWebApplicationFactory.TestAuthSecret, DateTime.UtcNow.AddMinutes(-10));

	public static string CreateTokenWithInvalidSignature() =>
		CreateToken("outra-chave-secreta-diferente-com-32-chars!!", DateTime.UtcNow.AddMinutes(5));

	private static string CreateToken(string secret, DateTime expires)
	{
		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
		var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

		var token = new JwtSecurityToken(
			issuer: CustomWebApplicationFactory.TestAuthIssuer,
			audience: CustomWebApplicationFactory.TestAuthAudience,
			claims: [new Claim(ClaimTypes.NameIdentifier, "integration-tests")],
			expires: expires,
			signingCredentials: credentials);

		return new JwtSecurityTokenHandler().WriteToken(token);
	}
}
