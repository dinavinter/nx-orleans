using System.Reflection;
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



    app.MapPost("/{id}", async (AccountUpdateRequest request)=>
    {
      // update account.

    });

    app.MapPost("/new", async (AccountCreateRequest request)  =>
    {

    }).RequireAuthorization("account_creation");;

    app.MapPost("/", async (AccountUpsertRequest request, string apiKey, [FromServices]IGrainFactory grainFactory)  =>
    {
       // upsert account



    })
    .RequireAuthorization("account_upsert")
    .WithName("AccountUpsert")
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
  public static void PopulateMetadata(ParameterInfo parameter, EndpointBuilder builder)
  {
    builder.Metadata.Add(new ConsumesAttribute(typeof(AccountUpsertRequest),  "application/json" ){IsOptional = false});
  }
}

