using System.IO.Compression;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Orleans.Contrib.UniversalSilo;

namespace api;

/// <summary>
/// Override methods in this class to take over how the web-api host is configured
/// </summary>
static class WebApiConfigurator
{
  public static readonly OpenApiInfo apiInfo =
    new OpenApiInfo
    {
      Version = "v1",                                                                // revision this appropriately
      Title = "My Orleans API",                                                      // title this application
      Description = "An application with an Orleans backend and a WebAPI interface", // describe this application
      TermsOfService = new Uri("http://127.0.0.1"),                                  // replace with <Your TOS Uri>
      Contact = new OpenApiContact()
      {
        Name = "A Sagacious Developer",                                            // replace with <Your Name>
        Email = "<Your Email>",                                                    // replace with <Your Email>
        Url = new Uri("http://127.0.0.1"),                                         // replace with <Your Uri>
      },
      License = new OpenApiLicense
      {
        Name = "A generous license",                                               // replace with <Your License Name>
        Url = new Uri("http://127.0.01"),                                          // replace with <Your License Uri>
      },
    };

  private static bool useHttpsRedirection = false;

  public static Dictionary<HealthStatus, int> healthResultStatusCodes = new Dictionary<HealthStatus, int>()
  {
    [HealthStatus.Healthy  ] = StatusCodes.Status200OK,
    [HealthStatus.Degraded ] = StatusCodes.Status200OK,
    [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
  };



  public static void AddApiServices(this IServiceCollection services )
  {

    services
      .AddControllers()
      .AddJsonOptions(options =>
      {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.WriteIndented = true;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.AllowTrailingCommas = true;
        options.JsonSerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;
        options.JsonSerializerOptions.AllowTrailingCommas = true;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
      });

    services
      .AddHealthChecks()
      .AddCheck<HealthChecks.ClusterHealthCheck>(nameof(HealthChecks.ClusterHealthCheck))
      .AddCheck<HealthChecks.LocalHealthCheck>(nameof(HealthChecks.LocalHealthCheck))
      .AddCheck<HealthChecks.StorageHealthCheck>(nameof(HealthChecks.StorageHealthCheck))
      .AddCheck<HealthChecks.SiloHealthCheck>(nameof(HealthChecks.SiloHealthCheck));

    services
      .AddResponseCompression()
      .Configure((BrotliCompressionProviderOptions options) => { options.Level = CompressionLevel.Optimal; })
      .Configure((GzipCompressionProviderOptions options) => { options.Level = CompressionLevel.Optimal; });

  }
}