using System.Threading.Tasks;
using grains.guest.contract.result;
using Microsoft.AspNetCore.Http.HttpResults;
using Orleans;

namespace grains.guest.contract;

public interface IAuthGuest : IGrainWithGuidKey
{
  Task<Results<Ok<AuthenticationConnectState>, Ok<AuthenticationChallengeState>, ValidationProblem>> Auth(GuestAuthenticationRequest request);
  Task<AuthenticationState> AuthenticateAsync(GuestAuthenticationProperties properties);

}
