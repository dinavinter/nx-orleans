using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using System.Text.Json.Serialization;
using Json.Schema.Generation;
using Microsoft.IdentityModel.JsonWebTokens;

namespace grains.guest.contract.result;

public record AuthenticationConnectState : AuthenticationState
{
  [JsonPropertyName("authentication_token"), JsonPropertyOrder(10)]
  public AuthToken AuthToken { get; set; }

  public AuthenticationConnectState(GuestAuthenticationProperties properties) : base("connect", new GuestChallenge(),  properties)
  {

    AuthToken = new AuthToken(properties.Context.User, properties.Context.Tenant, properties.Context.Device,
      new AuthDetails()
      {
        Method = "guest",
        Channel = "identifier"
      });


  }

  public record GuestChallenge: Challenge
  {
    [JsonPropertyName("acr_values")]
    [Const("urn:gigya:guest")]
    public string AcrValues { get; set; } = "urn:gigya:guest";

    [JsonPropertyName("type")]
    [Const("identification")]
    public string Type { get; set; } = "identification";

    [JsonPropertyName("handled")]
    public DateTimeOffset Handled{ get; set; } = DateTimeOffset.Now;


  }



}

public record AuthDetails
{
  public string Method { get; set; }
  public string Channel { get; set; }
}
