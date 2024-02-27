using api;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using static api.WebApiConfigurator;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseOrleans(new SiloConfigurator().ConfigurationFunc);

builder.Services.AddControllers();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();
builder.Services.AddApiServices();

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


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


