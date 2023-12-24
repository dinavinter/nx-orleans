using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace grains.guest.contract.result;

[JsonConverter(typeof(AuthTokenJwtConverter))]
public record AuthToken(User User, TenantDetails Tenant, DeviceDetails Device, AuthDetails AuthDetails)
{
  [JsonIgnore]
  public string Token => new JsonWebTokenHandler().CreateToken(CreateTokenDescriptor());

  public SecurityTokenDescriptor CreateTokenDescriptor()
  {
    return JwsSettings().CreateTokenDescriptor(AdditionalHeaderClaims(), Claims());


    JwsSettings JwsSettings() => new()
    {
      Issuer = "https://oauth2.gigya.com",
      ExpiresIn = TimeSpan.FromMinutes(5),
      CompressionAlgorithm = CompressionAlgorithms.Deflate,
      TokenType = "id+jws",
       SigningCredentials =
        new SigningCredentials(new ECDsaSecurityKey(ECDsa.Create(ECCurve.NamedCurves.nistP256)), "ES256")
    };

    Dictionary<string, object> AdditionalHeaderClaims() => new()
    {
      ["dc"] = User.Dc,
      ["tid"] = Tenant.Tid,
      ["cid"] = "<context-id (random consist between two tokens)>",
      ["jti"] = RandomNumberGenerator.GetInt32(0, int.MaxValue)
    };

    ClaimsIdentity Claims() => new(new List<Claim>()
      {
        new("amr", AuthDetails.Method),
        new("user", JsonSerializer.Serialize(User), JsonClaimValueTypes.Json),
        new("tenant", JsonSerializer.Serialize(Tenant), JsonClaimValueTypes.Json),
        new("device", JsonSerializer.Serialize(Device), JsonClaimValueTypes.Json),
        new("auth", JsonSerializer.Serialize(AuthDetails), JsonClaimValueTypes.Json),

      }, $"urn:guest:identifier:{User.Format}", "guest", "guest");
  }


}


public class AuthTokenJwtConverter: JsonConverter<AuthToken>
{
  public override AuthToken Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    var token=  new JsonWebTokenHandler().ReadJsonWebToken(reader.GetString() );
    var auth= new AuthToken(token.GetPayloadValue<User>("user"),
      token.GetPayloadValue<TenantDetails>("tenant"),
      token.GetPayloadValue<DeviceDetails>("device"),
      token.GetPayloadValue<AuthDetails>("auth"));
    return auth;

  }



  public override void Write(Utf8JsonWriter writer, AuthToken value, JsonSerializerOptions options)
  {
    writer.WriteStringValue(new JsonWebTokenHandler().CreateToken(value.CreateTokenDescriptor()));

  }
}
