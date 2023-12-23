using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;
using Json.Schema.Generation;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using JsonClaimValueTypes = Microsoft.IdentityModel.JsonWebTokens.JsonClaimValueTypes;

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

[JsonConverter(typeof(JwsJsonConverter))]
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
      Audience = $"https://accounts.{User.Dc}.gigya.com/identity.connect",
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





public class JwsSettings
{
  public  string Audience { get; set; }
  public string Issuer { get; set; }
  public SigningCredentials SigningCredentials { get; set; }
  public TimeSpan ExpiresIn { get; set; }
  public string CompressionAlgorithm { get; set; }
  public string TokenType { get; set; }


  public SecurityTokenDescriptor CreateTokenDescriptor( Dictionary<string, object> additionalHeaderClaims, ClaimsIdentity subject)
  {
    return new SecurityTokenDescriptor()
    {
      Issuer = Issuer,
      Audience = Audience,
      Expires = DateTime.Now.Add(ExpiresIn),
      CompressionAlgorithm = CompressionAlgorithm,
      TokenType = TokenType,
      SigningCredentials = SigningCredentials,
      AdditionalHeaderClaims = additionalHeaderClaims,
      Subject = subject
    };
  }


}

public class JwsJsonConverter: JsonConverter<AuthToken>
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
