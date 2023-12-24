using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using grains.guest.contract;
using grains.guest.contract.result;
using grains.schema.contract;
using Json.More;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.JsonWebTokens;
using Orleans;
using Orleans.Concurrency;

namespace SchemaLand.Api.Controllers;

public static class AuthApi
{
  /// <summary>
  /// guest authentication API , prepare a connection token for connect api, if the payload is allowed for guest also an authentication token is returned, otherwise a challenge is returned
  /// </summary>
  public static void MapAuthGuest(this WebApplication app)
  {
    var auth = app.MapGroup("auth").WithOpenApi();
    auth.AllowAnonymous();
    auth.MapPost("guest", GuestAuthentication)
      .WithName("auth.guest")
      .WithOpenApi()
      .Accepts<GuestAuthenticationRequest>("application/form-urlencoded")
      .Produces<AuthenticationConnectState>(200)
      .Produces<AuthenticationChallengeState>(206)
      .Produces<HttpValidationProblemDetails>(400)
      .WithOpenApi();
  }

  public static async Task<Results<Ok<AuthenticationConnectState>, Ok<AuthenticationChallengeState>, ValidationProblem>>
    GuestAuthentication(GuestAuthenticationRequest request, [FromServices] IClusterClient clusterClient)
  {
    var authProps = new GuestAuthenticationProperties(request);

    var validationResult = await clusterClient
      .GetGrain<IAccountSchemaGrain>("<site-id>")
      .ValidateAsync(new Immutable<JsonElement>(request.ToJsonDocument().RootElement));

    if (validationResult is { IsValid: false })
    {
      return TypedResults.ValidationProblem(validationResult.Errors);
    }

    var guestGrain = clusterClient.GetGrain<IAuthGuest>(Guid.NewGuid());
    var authentication = await guestGrain.AuthenticateAsync(authProps);
    return authentication switch
    {
      AuthenticationConnectState connect => TypedResults.Ok(connect),
      AuthenticationChallengeState challenge => TypedResults.Ok(challenge),
      _ => throw new InvalidOperationException()
    };
  }
}
