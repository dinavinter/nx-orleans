using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace grains.guest.contract;


public record State(string Value, object Context);

[JsonPolymorphic(TypeDiscriminatorPropertyName = "value")]
[JsonDerivedType(typeof(AuthenticationConnectState), typeDiscriminator: "connect")]
[JsonDerivedType(typeof(AuthenticationChallengeState), typeDiscriminator: "challenge")]
public record AuthenticationState(string State, Challenge Challenge, GuestAuthenticationProperties Properties):GuestAuthenticateResult(State, Challenge, Properties)
{
  [JsonPropertyName("state") ]
  public string State { get; set; } = State;

  [JsonIgnore]
  private GuestAuthenticationProperties Properties { get; init; } = Properties;

  [JsonPropertyName("challenge")]
  public Challenge Challenge { get; init; } = Challenge;




}


public record AuthenticationConnectState(GuestAuthenticationProperties Properties)
  : AuthenticationState("connect", new GuestChallenge(),  Properties)
{
  public record GuestChallenge: Challenge
  {
    [JsonPropertyName("acr_values")]
    public string AcrValues { get; set; } = "urn:gigya:gust";

    [JsonPropertyName("type")]
    public string Type { get; set; } = "identification";

    [JsonPropertyName("handled")]
    public DateTimeOffset Handled{ get; set; } = DateTimeOffset.Now;
   }
  [JsonIgnore]
  private GuestAuthenticationProperties Properties { get; init; } = Properties;

}

public record AuthenticationChallengeState(GuestAuthenticationProperties Properties):  AuthenticationState("challenge", new AuthenticationChallenge(), Properties)
{
  public record AuthenticationChallenge:Challenge
  {
    [JsonPropertyName("acr_values")]
    public string AcrValues { get; set; } = "urn:gigya:loa10";

    [JsonPropertyName("type")]
    public string Type { get; set; } = "authentication";

    [JsonPropertyName("prompt")]
    public string Prompt { get; set; } = "verify";

  }

  [JsonIgnore]
  private GuestAuthenticationProperties Properties { get; init; } = Properties;
}

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(AuthenticationChallengeState.AuthenticationChallenge), typeDiscriminator: "authentication")]
[JsonDerivedType(typeof(AuthenticationConnectState.GuestChallenge), typeDiscriminator: "identification")]
public record Challenge();






