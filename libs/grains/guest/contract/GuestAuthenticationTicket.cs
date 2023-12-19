using System.Text.Json.Serialization;

namespace grains.guest.contract;


public record GuestAuthenticationTicket(GuestAuthenticationProperties Properties, AuthenticationState State)
{

  //
  // public static GuestAuthenticationTicket RequireAuthentication( GuestAuthenticationProperties properties)
  // {
  //   return new GuestAuthenticationTicket(properties, new AuthenticationChallengeState());
  //
  // }
  // public static GuestAuthenticationTicket ReadyToConnect( GuestAuthenticationProperties properties)
  // {
  //   return new GuestAuthenticationTicket(properties, new AuthenticationConnectState());
  // }


}

