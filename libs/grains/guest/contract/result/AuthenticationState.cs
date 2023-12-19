using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json.Serialization;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace grains.guest.contract.result;


public record State(string Value, object Context);

[JsonPolymorphic(TypeDiscriminatorPropertyName = "value")]
[JsonDerivedType(typeof(AuthenticationConnectState), typeDiscriminator: "connect")]
[JsonDerivedType(typeof(AuthenticationChallengeState), typeDiscriminator: "challenge")]
public record AuthenticationState
{
  protected AuthenticationState(string State, Challenge challenge, GuestAuthenticationProperties properties)
  {
    this.State = State;
    ConnectToken = GetConnectToken(State, challenge, properties);

  }


  [JsonPropertyName("connection")]
  public string ConnectToken { get; set; }


  [JsonPropertyName("state") ]
  public string State { get; set; }



  static string GetConnectToken(string state, Challenge challenge, GuestAuthenticationProperties properties)
  {
    var Key = ECDsa.Create(ECCurve.NamedCurves.nistP256);

    return new JsonWebTokenHandler().CreateToken(new SecurityTokenDescriptor()
    {
      Issuer = "https://oauth2.gigya.com",
      Audience = $"https://accounts.{properties.Context.User.Dc}.gigya.com/identity.connect",

      SigningCredentials = new SigningCredentials(new ECDsaSecurityKey(Key), "ES256"),
      Expires = DateTime.Now.AddMinutes(5),
      CompressionAlgorithm = CompressionAlgorithms.Deflate,
      TokenType = "connect+jws",
      AdditionalHeaderClaims = new Dictionary<string, object>()
      {
        ["sub"] = properties.Context.User.Id,
        ["dc"] = properties.Context.User.Dc,
        ["tid"] = properties.Context.Tenant.Tid,
        ["cid"] = "<context-id (random consist between two tokens)>",
        ["jti"] = RandomNumberGenerator.GetInt32(0, int.MaxValue),
        //todo: check if to add nonce
        // ["nonce"] = "<nonce>?",

      },
      Subject = new ClaimsIdentity(new[]
      {
        new Claim("context", JsonSerializer.Serialize(properties.Context), JsonClaimValueTypes.Json),
        new Claim("state", state),
        new Claim("states", JsonSerializer.Serialize(new
        {
          challenge,
          connect = properties.ConnectDetails,

        }), JsonClaimValueTypes.Json),

      }, "guest")
    });
  }

}

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(AuthenticationChallengeState.AuthenticationChallenge), typeDiscriminator: "authentication")]
[JsonDerivedType(typeof(AuthenticationConnectState.GuestChallenge), typeDiscriminator: "identification")]
public record Challenge();




