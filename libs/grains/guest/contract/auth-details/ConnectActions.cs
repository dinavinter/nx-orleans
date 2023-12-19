using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace grains.guest.contract;

[JsonSerializable(typeof(ConnectAction))]
[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(CredentialUpdate), typeDiscriminator: "urn:account:update:credential")]
[JsonDerivedType(typeof(DataUpdate), typeDiscriminator: "urn:account:update")]
[JsonDerivedType(typeof(IdentityUpdate), typeDiscriminator: "urn:account:update:identity")]
public record ConnectAction
{
  [JsonExtensionData] public IDictionary<string, JsonElement> Data { get; set; }
}

public record DataUpdate : ConnectAction;

public record CredentialUpdate(string Format, string Value) : ConnectAction
{
  [JsonPropertyName("format")] public string Format { get; set; } = Format;

  [JsonPropertyName("value")] public string Value { get; set; } = Value;
}

public record IdentityUpdate : ConnectAction
{
  [JsonPropertyName("provider")]
  public string Provider { get; set; }

  /// <summary>
  /// the provider id, no need to specify if the provider is the site
  /// </summary>
  [JsonPropertyName("id")]
  [CanBeNull]
  public string Id { get; set; }

  [JsonPropertyName("profile")] public JsonElement? Profile { get; set; }

  [JsonPropertyName("data")] public JsonElement? Data { get; set; }
}

public class ConnectDetails : Dictionary<string, ConnectAction>
{
  public void Credential(UserId id)
  {
    this["urn:account:update:credential"] = new CredentialUpdate(id.Format, id.Value);
  }

  public void Identity(JsonElement? profile, JsonElement? data)
  {
    if (profile is not null || data is not null)
      this["urn:account:update:identity"] = new IdentityUpdate()
      {
        Profile = profile,
        Data = data,
        Provider = "site"
      };
  }

  public void UserInfo(IDictionary<string, JsonElement>? json)
  {
    if (json is not null)
    {
      this["urn:account:update"] = new DataUpdate
      {
        Data = json
      };
    }
  }
}
