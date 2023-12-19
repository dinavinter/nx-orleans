using System.Text.Json.Serialization;

namespace grains.guest.contract.result;

public record AuthenticationChallengeState(GuestAuthenticationProperties Properties):  AuthenticationState("challenge",TheChallenge, Properties)
{
  public static AuthenticationChallenge TheChallenge = new AuthenticationChallenge();

  [JsonPropertyName("challenge")]
  public Challenge Challenge { get; init; } = TheChallenge;


  public record AuthenticationChallenge :Challenge
  {
    [JsonPropertyName("acr_values")]
    public string AcrValues { get; set; } = "urn:gigya:loa10";

    [JsonPropertyName("type")]
    public string Type { get; set; } = "authentication";

    [JsonPropertyName("prompt")]
    public string Prompt { get; set; } = "verify";


  }


}
