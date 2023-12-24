using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace grains.guest.contract.result;

[JsonConverter(typeof(JsonConverter))]
public record ConnectToken(string State, Context Context, Challenge Challenge, ConnectDetails Connect)
{
  public string Token => new JsonWebTokenHandler().CreateToken(CreateTokenDescriptor());

  SecurityTokenDescriptor CreateTokenDescriptor()
  {
    return JwsSettings().CreateTokenDescriptor(AdditionalHeaderClaims(), Claims());

    JwsSettings JwsSettings() => new()
    {
      Audience = $"https://accounts.{Context.User.Dc}.gigya.com/identity.connect",
      CompressionAlgorithm = CompressionAlgorithms.Deflate,
      ExpiresIn = TimeSpan.FromMinutes(5),
      Issuer = "https://oauth2.gigya.com",
      SigningCredentials =
        new SigningCredentials(new ECDsaSecurityKey(ECDsa.Create(ECCurve.NamedCurves.nistP256)), "ES256"),
      TokenType = "connect+jws"

    };

    Dictionary<string, object> AdditionalHeaderClaims() => new()
    {
      ["sub"] = Context.User.Id,
      ["dc"] = Context.User.Dc,
      ["tid"] = Context.Tenant.Tid,
      ["cid"] = "<context-id (random consist between two tokens)>",
      ["jti"] = RandomNumberGenerator.GetInt32(0, int.MaxValue),
    };

    ClaimsIdentity Claims() => new(new[]
    {
      new Claim("context", JsonSerializer.Serialize(Context), JsonClaimValueTypes.Json),
      new Claim("state", State),
      new Claim("challenge", JsonSerializer.Serialize(Challenge), JsonClaimValueTypes.Json),
      new Claim("connect", JsonSerializer.Serialize(Connect), JsonClaimValueTypes.Json)
    }, "guest");
  }
  static ConnectToken FromToken(JsonWebToken jsonWebToken)
  {
    return new ConnectToken(jsonWebToken.GetPayloadValue<string>("state"),
      jsonWebToken.GetPayloadValue<Context>("context"),
      jsonWebToken.GetPayloadValue<Challenge>("challenge"),
      jsonWebToken.GetPayloadValue<ConnectDetails>("connect"));
  }

  public class JsonConverter : JsonConverter<ConnectToken>
  {
    public override ConnectToken Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)=>
      FromToken(new JsonWebTokenHandler().ReadJsonWebToken(reader.GetString()));


    public override void Write(Utf8JsonWriter writer, ConnectToken value, JsonSerializerOptions options)=>
      writer.WriteStringValue(new JsonWebTokenHandler().CreateToken(value.CreateTokenDescriptor()));

  }
}



