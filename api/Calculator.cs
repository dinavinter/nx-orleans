using grains.Contract;
using Microsoft.AspNetCore.Mvc;
using Orleans;

namespace api;

public static class Calculator
{
  public static void Endpoints(WebApplication app)
  {
    app.Map("/", () => Results.Redirect("/swagger"));


    app.MapGet("/calc/{id}",
        async ([FromServices] IClusterClient grainFactory, string id) =>
        await grainFactory.GetGrain<ICalculatorGrain>(id).Get())
      .WithOpenApi()
      .WithTags("calculator")
      .WithName("get")
      .WithDescription("Gets the current value of a calculator instance.");

    app.MapPost("/calc/{id}/add", async ([FromServices] IClusterClient grainFactory, string id, int value) =>
      await grainFactory.GetGrain<ICalculatorGrain>(id).Add(value))
      .WithOpenApi()
      .WithTags("calculator")
      .WithName("add")
      .WithDescription("Adds the given value to the current value in a calculator instance.");

    app.MapPost("/calc/{id}/subtract",
        async ([FromServices] IClusterClient grainFactory, string id, int value) =>
        await grainFactory.GetGrain<ICalculatorGrain>(id).Subtract(value))
      .WithOpenApi()
      .WithTags("calculator")
      .WithName("subtract")
      .WithDescription("Substructs the given value from the current value in a calculator instance.");

    app.MapPost("/calc/{id}/undo",
        async ([FromServices] IClusterClient grainFactory, string id) =>
        await grainFactory.GetGrain<ICalculatorGrain>(id).Undo())
      .WithOpenApi()
      .WithTags("calculator")
      .WithName("undo")
      .WithDescription("""
                       Undo the last operation in a calculator instance

                       #### Usage example:
                         ```js
                         await fetch('/calc/my-calc/add', { method: 'POST', body: '4' });
                         //=> 4
                         await fetch('/calc/my-calc/add', { method: 'POST', body: '5' });
                         //=> 9
                         await fetch('/calc/my-calc/undo', { method: 'POST' });
                          //=> 4
                         ```
                       """);
  }
}
