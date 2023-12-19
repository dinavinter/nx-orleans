using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace SchemaLand.Api.Controllers;

public class MapExampleController
{
  private readonly ILogger<MapExampleController> _logger;

  public MapExampleController(ILogger<MapExampleController> logger)
  {
    _logger = logger;
  }

  public static void Map(WebApplication app)
  {
    app.MapGet("/posts/{*rest}", (string rest) => $"Routing to {rest}");
    app.MapGet("/todos/{id:int}", (int id) => $"todo {id}") ;
    app.MapGet("/todos/{text}", (string text) => $"todo {text}");
    app.MapGet("/posts/{slug:regex(^[a-z0-9_-]+$)}", (string slug) => $"Post {slug}");
    var all = app.MapGroup("").WithOpenApi();
    var org = all.MapGroup("{org}");

    app.MapGet("/", async context =>
    {
      // Get all todo items
      await context.Response.WriteAsJsonAsync(new { Message = "All todo items" });
    });

    app.MapGet("/users/{userId}/books/{bookId}",
      (int userId, int bookId) => $"The user id is {userId} and book id is {bookId}");


    app.MapGet("/{id}", async context =>
    {
      // Get one todo item
      await context.Response.WriteAsJsonAsync(new { Message = "One todo item" });
    });


  }


}
