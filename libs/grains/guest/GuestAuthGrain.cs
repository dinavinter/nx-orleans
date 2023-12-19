using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using grains.guest.contract;
using grains.guest.contract.mock;
using grains.guest.contract.result;
using grains.schema.contract;
using Json.More;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Orleans;
using Orleans.Concurrency;

namespace grains.guest
{
  [Reentrant, StatelessWorker]
    public class GuestAuthGrain : Grain, IAuthGuest
    {
      private GuestAuthenticationSchema _schema = new GuestAuthenticationSchema();

      /// <inheritdoc />
      public async Task<Results<Ok<AuthenticationConnectState>, Ok<AuthenticationChallengeState>, ValidationProblem>> Auth(GuestAuthenticationRequest request)
      {
        var properties = new GuestAuthenticationProperties(request);
        var result = await ValidateAsync(request.ToJsonDocument().AsImmutable());
        if (!result.IsValid)
        {
          return TypedResults.ValidationProblem(result.Errors);
        }

        var state = await AuthenticateAsync(properties);
        return state switch
        {
          AuthenticationConnectState connect => TypedResults.Ok(connect),
          AuthenticationChallengeState challenge => TypedResults.Ok(challenge),
        };


      }

      public async Task<SchemaValidationResult> ValidateAsync(Immutable<JsonDocument> instance)
      {
        return await _schema.ValidateAsync(instance.Value.RootElement);
      }

       public async Task<AuthenticationState> AuthenticateAsync(GuestAuthenticationProperties properties)
      {

          var result = await ValidateAsync(properties.Request.ToJsonDocument().AsImmutable());

          return result switch
          {
            { IsValid: true } =>  new AuthenticationConnectState(properties),
            { IsValid: false } => new AuthenticationChallengeState(properties),
          };
      }
    }
}
