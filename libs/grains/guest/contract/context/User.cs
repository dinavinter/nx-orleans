using System.Text.Json.Serialization;

namespace grains.guest.contract;

[JsonSerializable(typeof(User))]
public record User(string Format, string Dc, string Id)
{
  [JsonPropertyName("format")] public string Format { get; set; } = Format;
  [JsonPropertyName("dc")] public string Dc { get; set; } = Dc;
  [JsonPropertyName("id")] public string Id { get; set; } = Id;


  public record EmailUsers(string Email, string Dc) : User("email", Dc, Email);

  public record PhoneNumberUser(string Phone, string Dc) : User("phonenumber", Dc, Phone);

  public record Social(string Provider, string ProviderUid, string Dc) : User("phonenumber", Dc, $"{Provider}:{ProviderUid}");

}
