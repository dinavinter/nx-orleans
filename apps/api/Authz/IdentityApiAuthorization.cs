using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace api.Authz;

public static class IdentityApiAuthorization
{

 public static void AddGigyaBearerAuthentication(this AuthenticationBuilder authentication)
  {
    authentication
      .AddJwtBearer()
      .AddJwtBearer("auth.gigya.com", options =>
      {
        options.Authority = "https://auth.gigya.com";
        options.Audience = "https://accounts.gigya,com/identities";
        options.RequireHttpsMetadata = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
          ValidateIssuer = true,
          ValidateAudience = true,
          ValidateLifetime = true,
          ValidateIssuerSigningKey = true,
          ValidIssuer = "https://auth.gigya.com",
          ValidAudience = "https://accounts.gigya.com/identities",
          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("secret"))
        };
        options.MapInboundClaims = false;
      });
  }

  public static void AddAccountAuthorizationPolicies(this AuthorizationBuilder authorizationBuilder)
  {

    authorizationBuilder
      .AddPolicy("account_creation", policy =>
        policy
          .RequireAssertion(context =>
            context.User.HasClaim("account/creation", "allowed")
          ));


    authorizationBuilder
      .AddPolicy("account_upsert", policy =>
        policy
          .RequireAssertion(context =>
            context.User.HasClaim("account/update", "allowed")
          ));
  }

}
