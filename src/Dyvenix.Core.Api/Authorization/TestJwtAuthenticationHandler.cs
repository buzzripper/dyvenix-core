using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Dyvenix.Core.Api.Authorization;

public sealed class TestJwtAuthenticationHandler(
	IOptionsMonitor<AuthenticationSchemeOptions> options,
	ILoggerFactory logger,
	UrlEncoder encoder)
	: AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
	public const string SchemeName = "TestJwt";

	protected override Task<AuthenticateResult> HandleAuthenticateAsync()
	{
		if (!AuthenticationHeaderValue.TryParse(Request.Headers.Authorization, out var header)
			|| !string.Equals(header.Scheme, "Bearer", StringComparison.OrdinalIgnoreCase)
			|| string.IsNullOrWhiteSpace(header.Parameter))
		{
			return Task.FromResult(AuthenticateResult.NoResult());
		}

		JwtSecurityToken token;
		try
		{
			token = new JwtSecurityTokenHandler().ReadJwtToken(header.Parameter);
		}
		catch (Exception ex)
		{
			return Task.FromResult(AuthenticateResult.Fail(ex));
		}

		var identity = new ClaimsIdentity(token.Claims, Scheme.Name);
		var principal = new ClaimsPrincipal(identity);
		var ticket = new AuthenticationTicket(principal, Scheme.Name);

		return Task.FromResult(AuthenticateResult.Success(ticket));
	}
}
