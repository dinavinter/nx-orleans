using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace grains.guest.contract;

public record GuestAuthenticateResult
{



  public GuestAuthenticateResult(string state,  Challenge challenge, GuestAuthenticationProperties properties)
  {
    ECDsa Key= ECDsa.Create(ECCurve.NamedCurves.nistP256);

    Connect = properties.ConnectDetails;
    User = properties.Subject.User;
    SubjectToken = new JsonWebTokenHandler().CreateToken(new SecurityTokenDescriptor()
    {
      Issuer = "https://oauth2.gigya.com",
      SigningCredentials = new SigningCredentials(new ECDsaSecurityKey(Key), "ES256"),
      Expires = DateTime.Now.AddMinutes(5),
      CompressionAlgorithm = CompressionAlgorithms.Deflate,
      TokenType = "id+jws",
      AdditionalHeaderClaims = new Dictionary<string, object>()
      {
        ["sub"] = properties.Subject.User.Id,
        ["dc"] = properties.Subject.User.Dc,
        ["tid"] = properties.Subject.Tenant.Tid,
        ["cid"] = "<context-id (random consist between two tokens)>",
        ["jti"] = RandomNumberGenerator.GetInt32(0, int.MaxValue)
      },
      Subject = new ClaimsIdentity(new List<Claim>()
        {
          new("amr", "guest"),
          new("acr", "urn:gigya:guest"),
          new("subject", JsonSerializer.Serialize(properties.Subject), JsonClaimValueTypes.Json),
          new("auth", JsonSerializer.Serialize(new
          {
            method = "guest",
            channel = "identifier"
          }), JsonClaimValueTypes.Json),

        }, $"urn:guest:identifier:{User.Format}", "guest", "guest")
    });

    ConnectToken = new JsonWebTokenHandler().CreateToken(new SecurityTokenDescriptor()
    {
      Issuer = "https://oauth2.gigya.com",
      Audience = $"https://accounts.{properties.Subject.User.Dc}.gigya.com/identity.connect",
      SigningCredentials = new SigningCredentials(new ECDsaSecurityKey(Key), "ES256"),
      Expires = DateTime.Now.AddMinutes(5),
      CompressionAlgorithm = CompressionAlgorithms.Deflate,
      TokenType = "connect+jws",
      AdditionalHeaderClaims = new Dictionary<string, object>()
      {
        ["sub"] = properties.Subject.User.Id,
        ["dc"] = properties.Subject.User.Dc,
        ["tid"] = properties.Subject.Tenant.Tid,
        ["cid"] = "<context-id (random consist between two tokens)>",
        ["jti"] = RandomNumberGenerator.GetInt32(0, int.MaxValue) ,
        //todo: check if to add nonce
        // ["nonce"] = "<nonce>?",

      },
      Subject = new ClaimsIdentity(new[]
      {
        new Claim("subject", JsonSerializer.Serialize(properties.Subject), JsonClaimValueTypes.Json),
        new Claim("state", state),
        new Claim("challenge", JsonSerializer.Serialize(challenge), JsonClaimValueTypes.Json),
        new Claim("connect", JsonSerializer.Serialize(properties.ConnectDetails), JsonClaimValueTypes.Json),
      }, "guest")
    });

  }


  [JsonPropertyName("connect_token")]
  public string ConnectToken { get; set; }

  [JsonPropertyName("subject_token")]
  public string SubjectToken { get; set; }

  [JsonPropertyName("user")]
  public User User { get; set; }



  [JsonPropertyName("connect")]
  public ConnectDetails Connect { get; set; }

}


