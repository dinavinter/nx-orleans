using System.Text.Json.Serialization;

namespace grains.guest.contract.result;


public record State(string Value, object Context);

[JsonPolymorphic(TypeDiscriminatorPropertyName = "value")]
[JsonDerivedType(typeof(AuthenticationConnectState), typeDiscriminator: "connect")]
[JsonDerivedType(typeof(AuthenticationChallengeState), typeDiscriminator: "challenge")]
public record AuthenticationState
{
  protected AuthenticationState(string State, Challenge challenge, GuestAuthenticationProperties properties)
  {
    this.State = State;
    ConnectToken = GetConnectToken(State, challenge, properties);

  }


  [JsonPropertyName("connect_token"), JsonPropertyOrder(9)]
  public ConnectToken ConnectToken { get; set; }


  [JsonPropertyName("state" ), JsonPropertyOrder(0) ]
  public string State { get; set; }



  static ConnectToken GetConnectToken(string state, Challenge challenge, GuestAuthenticationProperties properties)
  {
    return new ConnectToken(state, properties.Context, challenge,  properties.ConnectDetails);
  }

}

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(AuthenticationChallengeState.AuthenticationChallenge), typeDiscriminator: "authentication")]
[JsonDerivedType(typeof(AuthenticationConnectState.GuestChallenge), typeDiscriminator: "identification")]
public record Challenge();
