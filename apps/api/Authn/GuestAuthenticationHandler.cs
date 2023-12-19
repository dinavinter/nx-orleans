using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;



public class GuestAuthenticationHandler: AuthenticationHandler<AuthenticationSchemeOptions>
{
  /// <inheritdoc />
  public GuestAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
  {
  }

  /// <inheritdoc />
  protected override Task<AuthenticateResult> HandleAuthenticateAsync()
  {
      var claims = new List<Claim>          {
      new Claim(ClaimTypes.Name, "Guest"),
      new Claim(ClaimTypes.Role, "Guest"),
    };
    var identity = new ClaimsIdentity(claims, Scheme.Name);
    var principal = new ClaimsPrincipal(identity);
    var ticket = new AuthenticationTicket(principal, Scheme.Name);
    return Task.FromResult(AuthenticateResult.Success(ticket));
  }
}
