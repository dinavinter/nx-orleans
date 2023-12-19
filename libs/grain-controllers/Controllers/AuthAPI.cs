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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.JsonWebTokens;
using Orleans;
using Orleans.Concurrency;

namespace SchemaLand.Api.Controllers;

[ApiController]
[Route("api/auth")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
  public AuthController(
    ILogger<AuthController> logger,
    IClusterClient orleansClient)
  {
    OrleansClient = orleansClient;
    Logger = logger;
  }

  public IClusterClient OrleansClient { get; }
  public ILogger Logger { get; }


  /// <summary>
  /// guest authentication API , prepare a connection token for connect api, if the payload is allowed for guest also an authentication token is returned, otherwise a challenge is returned
  /// </summary>
   /// <remarks>
  /// Sample request:
  ///
  ///     POST /auth.guest
  ///     {
  ///        "id": {
  ///            "format": "email",
  ///            "email" : "email@mail.com"
  ///
  ///          },
  ///        "communication": {"news":{"status":"opt-in"}},
  ///        "preferences": {"tos":{"isConsentGranted":true}},
  ///        "profile":{"firstName":"Jon","lastName":"Smith"},
  ///        "data":{"campaign":"jon@fo"}
  ///     }
  ///
  /// </remarks>

  [HttpPost("guest")]
  [AllowAnonymous]
  [Consumes(typeof(GuestAuthenticationRequest), "application/form-urlencoded")]
  [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthenticationConnectState))]
  [ProducesResponseType(StatusCodes.Status206PartialContent, Type = typeof(AuthenticationChallengeState))]
  [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ValidationProblem))]
  public virtual async Task<Results<Ok<AuthenticationConnectState>, Ok<AuthenticationChallengeState>, ValidationProblem>> Guest(
    [FromBody] GuestAuthenticationRequest request)
  {
    var authProps = new GuestAuthenticationProperties(request);

    var validationResult = await OrleansClient
      .GetGrain<IAccountSchemaGrain>("<site-id>")
      .ValidateAsync(new Immutable<JsonElement>(request.ToJsonDocument().RootElement));

    if (validationResult is { IsValid: false })
    {
      return TypedResults.ValidationProblem(validationResult.Errors);
    }

    var guestGrain = OrleansClient.GetGrain<IAuthGuest>(Guid.NewGuid());
    var authentication = await guestGrain.AuthenticateAsync(authProps);
    return authentication switch
    {
      AuthenticationConnectState connect => TypedResults.Ok(connect),
      AuthenticationChallengeState challenge => TypedResults.Ok(challenge),
    };
  }
}
