using System.Reflection;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc;
using Orleans;

namespace SchemaLand.Api.Controllers;



public static class IdentityMapping
{
  public static void MapIdentities(this WebApplication app)
  {

    var accounts = app.MapGroup("{apiKey}/accounts").WithOpenApi();
    accounts.RequireAuthorization("account_update");


    app.MapPost("/", async (AccountUpsertRequest request, string apiKey, [FromServices]IGrainFactory grainFactory)  =>
    {
     })
    .RequireAuthorization("account_upsert")
    .WithName("identity.connect")
    .WithOpenApi();




  }
}

public class AccountUpdateRequest
{

}

public class AccountCreateRequest
{

}

public class AccountUpsertRequest:IEndpointParameterMetadataProvider
{
  [JsonPropertyName("connection")]
  public string ConnectionToken { get; set; }

  [JsonPropertyName("authentication")]
  public string AuthenticationToken { get; set; }
  public static void PopulateMetadata(ParameterInfo parameter, EndpointBuilder builder)
  {
    builder.Metadata.Add(new ConsumesAttribute(typeof(AccountUpsertRequest),  "application/json" ){IsOptional = false});
  }
}

