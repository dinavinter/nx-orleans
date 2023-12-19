using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using grains.utilities;

namespace grains.guest.contract;
public record AuthorizationDetail
{
  [System.Text.Json.Serialization.JsonExtensionData]
  public IDictionary<string, JsonElement> Data { get; set; }



}
public class AuthorizationDetails : List<AuthorizationDetail>
{
  public List<AuthorizationDetail> Elements { get; set; }


  public void CredentialValidated(UserId id)
  {
    Add(new CredentialUpdate(id.Format, id.Value));
  }

  public void IdentityValidated(JsonElement? Profile, JsonElement? Data)
  {
    if (Profile is not null || Data is not null)
      Add(new IdentityUpdate()
      {
        Profile = Profile,
        Data = Data,
        Provider = "site"
      });

  }

  public void UserInfoValidated(IDictionary<string, JsonElement>? json)
  {
    if (json is not null)
    {
      Add(new DataUpdate("accounts.setAccountInfo")
      {
        Data = json
      });
    }
  }

  [JsonSerializable(typeof(CredentialUpdate))]
public record CredentialUpdate(string Format, string Value) : DataUpdate("ids")
{
  [JsonPropertyName("format")] public string Format { get; set; } = Format;

  [JsonPropertyName("value")] public string Value { get; set; } = Value;
}

[JsonSerializable(typeof(Credential))]
public record Credential(string Format, string Dc, string Id)
{
  [JsonPropertyName("format")] public string Format { get; set; } = Format;
  [JsonPropertyName("dc")] public string Dc { get; set; } = Dc;
  [JsonPropertyName("id")] public string Id { get; set; } = Id;

  public record EmailCredentials(string Email, string Dc) : Credential("email", Dc, Email);

  public record PhoneNumberCredential(string Phone, string Dc) : Credential("phonenumber", Dc, Phone);

  public record Social(string Provider, string ProviderUid, string Dc) : Credential("phonenumber", Dc, $"{Provider}:{ProviderUid}");

}



/// <summary>
     /// </summary>
   [JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Serialization, WriteIndented = true, PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
   [JsonSerializable(typeof(DataUpdate))]
   [JsonPolymorphic(TypeDiscriminatorPropertyName = "resource")]
   [JsonDerivedType(typeof(IdentityUpdate), typeDiscriminator:"identity")]
   [JsonDerivedType(typeof(CommunicationUpdate), typeDiscriminator:"communication")]
   [JsonDerivedType(typeof(PreferenceUpdate), typeDiscriminator:"preference")]
   [JsonDerivedType(typeof(CredentialUpdate), typeDiscriminator:"credentials")]
    public record DataUpdate(string Resource):AuthorizationDetail
    {
      [Enum("identity", "communication", "preference", "credentials")]
      [System.Text.Json.Serialization.JsonIgnore]
      public string Resource { get; set; } = Resource;

    }


    public record IdentityUpdate() : DataUpdate("identity")
    {
      [JsonPropertyName("provider")]
      public string Provider {get; set;}

      [JsonPropertyName("id")]
      public string Id {get; set;}

      [JsonPropertyName("profile")]
      public JsonElement? Profile {get; set;}

      [JsonPropertyName("data")]
      public JsonElement? Data {get; set;}


    }

    public record PreferenceUpdate() : DataUpdate("preference");
    public record CommunicationUpdate() : DataUpdate("communication");

}
