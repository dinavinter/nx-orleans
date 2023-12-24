using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace grains.guest.contract.result;

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
