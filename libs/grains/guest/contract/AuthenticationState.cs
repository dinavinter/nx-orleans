using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using JsonSerializer = System.Text.Json.JsonSerializer;

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
  protected GuestAuthenticationProperties Properties { get; init; } = Properties;

  [JsonPropertyName("challenge")]
  public Challenge Challenge { get; init; } = Challenge;



}


public record AuthenticationConnectState : AuthenticationState
{
  [JsonPropertyName("authentication_token")]
  public string SubjectToken { get; set; }

  public AuthenticationConnectState(GuestAuthenticationProperties Properties) : base("connect", new GuestChallenge(Properties),  Properties)
  {
    Challenge = null;
    ECDsa Key= ECDsa.Create(ECCurve.NamedCurves.nistP256);
    SubjectToken = new JsonWebTokenHandler().CreateToken(new SecurityTokenDescriptor()
    {
      Issuer = "https://oauth2.gigya.com",
      SigningCredentials = new SigningCredentials(new ECDsaSecurityKey(Key), "ES256"),
      Expires = DateTime.Now.AddMinutes(5),
      CompressionAlgorithm = CompressionAlgorithms.Deflate,
      TokenType = "id+jws",
      AdditionalHeaderClaims = new Dictionary<string, object>()
      {
        ["sub"] = Properties.Context.User.Id,
        ["dc"] = Properties.Context.User.Dc,
        ["tid"] = Properties.Context.Tenant.Tid,
        ["cid"] = "<context-id (random consist between two tokens)>",
        ["jti"] = RandomNumberGenerator.GetInt32(0, int.MaxValue)
      },
      Subject = new ClaimsIdentity(new List<Claim>()
        {
          new("amr", "guest"),
          new("acr", "urn:gigya:guest"),
          new("user", JsonSerializer.Serialize(Properties.Context.User), JsonClaimValueTypes.Json),
          new("tenant", JsonSerializer.Serialize(Properties.Context.Tenant), JsonClaimValueTypes.Json),
          new ("device", JsonSerializer.Serialize(Properties.Context.Device), JsonClaimValueTypes.Json),
          new("auth", JsonSerializer.Serialize(new
          {
            method = "guest",
            channel = "identifier"
          }), JsonClaimValueTypes.Json),

        }, $"urn:guest:identifier:{Properties.Context.User.Format}", "guest", "guest")
    });
  }

  public record GuestChallenge: Challenge
  {
    public GuestChallenge(GuestAuthenticationProperties Properties)
    {

    }
    [JsonPropertyName("acr_values")]
    public string AcrValues { get; set; } = "urn:gigya:gust";

    [JsonPropertyName("type")]
    public string Type { get; set; } = "identification";

    [JsonPropertyName("handled")]
    public DateTimeOffset Handled{ get; set; } = DateTimeOffset.Now;


   }


}

public record AuthenticationChallengeState(GuestAuthenticationProperties Properties):  AuthenticationState("challenge",new AuthenticationChallenge(Properties), Properties)
{

  public record AuthenticationChallenge :Challenge
  {
    public AuthenticationChallenge(GuestAuthenticationProperties properties)
    {
       User = properties.Context.User;
     }

    [JsonPropertyName("acr_values")]
    public string AcrValues { get; set; } = "urn:gigya:loa10";

    [JsonPropertyName("type")]
    public string Type { get; set; } = "authentication";

    [JsonPropertyName("prompt")]
    public string Prompt { get; set; } = "verify";

    [JsonPropertyName("user")]
    public User User { get; set; }


  }


 }

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(AuthenticationChallengeState.AuthenticationChallenge), typeDiscriminator: "authentication")]
[JsonDerivedType(typeof(AuthenticationConnectState.GuestChallenge), typeDiscriminator: "identification")]
public record Challenge();






