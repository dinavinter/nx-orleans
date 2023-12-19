using System.Text.Json.Serialization;

namespace grains.guest.contract;

public record GuestAuthenticationProperties
{
  public GuestAuthenticationProperties(GuestAuthenticationRequest request)
  {
     Request = request;
     ConnectDetails.UserInfo(request.UserInfo);
     ConnectDetails.Credential(request.Id);
     ConnectDetails.Identity(request.Profile, request.Data);
     Context = new Context()
     {
        Tenant = new TenantDetails()
        {
            GroupId = request.GroupId,
            SiteId =  request.SiteId,
            PartnerId= request.PartnerId,
            TenantName = request.TenantId
        },
        User = new User(request.Id.Format, "us1", request.Id.Value),
        Device = new DeviceDetails()
        {
          Id = request.DeviceId,
          Ip = request.Ip,
          UserAgent = request.UserAgent
        }
     };
   }

  public GuestAuthenticationRequest Request { get; set; }


   [JsonPropertyName("context")]
   public Context Context { get; set; } = new Context();


   [JsonPropertyName("connect")]
   public ConnectDetails ConnectDetails { get; set; } = new ConnectDetails();




}

