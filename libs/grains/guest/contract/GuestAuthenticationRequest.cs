using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace grains.guest.contract;

public record GuestAuthenticationRequest
{
  [JsonPropertyName("id")]
  public UserId Id { get; set; }
  [JsonExtensionData] public IDictionary<string, JsonElement> UserInfo { get; set; }

  [JsonPropertyName("profile")]
  public JsonElement? Profile { get; set; }

  [JsonPropertyName("data")]
  public JsonElement? Data { get; set; }

  public string TenantId { get; set; } = "tnt-amsos";
  public ulong GroupId { get; set; } = 132132321;
  public ulong SiteId { get; set; } = 12112323;
  public uint PartnerId { get; set; } = 909;
  public string DeviceId { get; set; } = "device-123";
  public string Ip { get; set; } = "192.168.901.§";
  public string UserAgent { get; set; } = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36";

}

public class UserId
{
  [JsonPropertyName("value")]
  public string Value { get; set; }

  [JsonPropertyName("format")]
  public string Format { get; set; }

}
