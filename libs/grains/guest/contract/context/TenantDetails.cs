using System.Text.Json.Serialization;

namespace grains.guest.contract;

public class TenantDetails
{
  [JsonPropertyName("site-id")] public ulong SiteId { get; set; } = 11138781;
  [JsonPropertyName("group-id")] public ulong GroupId { get; set; } = 87000000;
  [JsonPropertyName("partner-id")] public uint PartnerId { get; set; } = 8901;
  [JsonPropertyName("tenant-id")] public string TenantName { get; set; } = "asmosys";
  [JsonIgnore]public string Tid => $"urn:gigya:tid:{TenantName}:{PartnerId}:{SiteId}";
}
