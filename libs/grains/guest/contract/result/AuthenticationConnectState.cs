using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;
using Json.Schema.Generation;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace grains.guest.contract.result;

public record AuthenticationConnectState : AuthenticationState
{
  [JsonPropertyName("authentication_token"), JsonPropertyOrder(10)]
  public string SubjectToken { get; set; }

  public AuthenticationConnectState(GuestAuthenticationProperties properties) : base("connect", new GuestChallenge(),  properties)
  {
    using ECDsa key= ECDsa.Create(ECCurve.NamedCurves.nistP256);
    SubjectToken = new JsonWebTokenHandler().CreateToken(new SecurityTokenDescriptor()
    {
      Issuer = "https://oauth2.gigya.com",
      SigningCredentials = new SigningCredentials(new ECDsaSecurityKey(key), "ES256"),
      Expires = DateTime.Now.AddMinutes(5),
      CompressionAlgorithm = CompressionAlgorithms.Deflate,
      TokenType = "id+jws",
      AdditionalHeaderClaims = new Dictionary<string, object>()
      {
        ["sub"] = properties.Context.User.Id,
        ["dc"] = properties.Context.User.Dc,
        ["tid"] = properties.Context.Tenant.Tid,
        ["cid"] = "<context-id (random consist between two tokens)>",
        ["jti"] = RandomNumberGenerator.GetInt32(0, int.MaxValue)
      },
      Subject = new ClaimsIdentity(new List<Claim>()
        {
          new("amr", "guest"),
          new("acr", "urn:gigya:guest"),
          new("user", JsonSerializer.Serialize(properties.Context.User), JsonClaimValueTypes.Json),
          new("tenant", JsonSerializer.Serialize(properties.Context.Tenant), JsonClaimValueTypes.Json),
          new ("device", JsonSerializer.Serialize(properties.Context.Device), JsonClaimValueTypes.Json),
          new("auth", JsonSerializer.Serialize(new
          {
            method = "guest",
            channel = "identifier"
          }), JsonClaimValueTypes.Json),

        }, $"urn:guest:identifier:{properties.Context.User.Format}", "guest", "guest")
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
