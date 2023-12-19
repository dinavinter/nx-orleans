using System.Text.Json.Serialization;
using Json.Schema.Generation;

namespace grains.guest.contract.result;

public record AuthenticationChallengeState :  AuthenticationState
{
  private static readonly AuthenticationChallenge TheChallenge = new AuthenticationChallenge();

  public AuthenticationChallengeState(GuestAuthenticationProperties properties) : base("challenge",TheChallenge, properties)
  {

  }

  [JsonPropertyName("challenge"), JsonPropertyOrder(1)]
  public Challenge Challenge { get; init; } = TheChallenge;



  public record AuthenticationChallenge :Challenge
  {
    [JsonPropertyName("acr_values")]
    [Const("urn:gigya:loa10")]
    public string AcrValues { get; set; } = "urn:gigya:loa10";

    [JsonPropertyName("type")]
    [Const("authentication")]
    public string Type { get; set; } = "authentication";

    [JsonPropertyName("prompt")]
    [Const("verify")]
    public string Prompt { get; set; } = "verify";


  }

}
