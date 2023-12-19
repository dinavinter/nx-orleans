using Microsoft.AspNetCore.Authentication;

public static class AuthNConfigurator
{

  public static void AddGuestAuthentication(this AuthenticationBuilder authenticationBuilder)
  {
    authenticationBuilder.AddScheme<AuthenticationSchemeOptions, GuestAuthenticationHandler>("guest", null);
  }


}
