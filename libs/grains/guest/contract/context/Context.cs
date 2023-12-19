using System.Text.Json.Serialization;

namespace grains.guest.contract;



public record Context
{
  [JsonPropertyName("user")]
  public User User{ get; set; }

  [JsonPropertyName("tenant")]
  public TenantDetails Tenant { get; set; }

  [JsonPropertyName("device")]
  public DeviceDetails Device { get; set; }

  [JsonIgnore]
  public string Sub=> User?.Id;

}
