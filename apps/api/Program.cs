using api.Authz;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using SchemaLand.Api.Controllers;
using static WebApiConfigurator;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseOrleans(new SiloConfigurator().ConfigurationFunc);

builder.Services.AddControllers();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();
builder.Services.AddApiServices();

var authorization = builder.Services.AddAuthorizationBuilder();
authorization.AddAccountAuthorizationPolicies();

var authentication = builder.Services.AddAuthentication();
authentication.AddGuestAuthentication();
authentication.AddGigyaBearerAuthentication();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRouting();

app.UseHealthChecks("/health",
  new HealthCheckOptions()
  {
    AllowCachingResponses = false,
    ResultStatusCodes = healthResultStatusCodes
  });

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
  app.UseHttpsRedirection();

}


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapIdentities();
app.MapAuthGuest();

app.Run();

