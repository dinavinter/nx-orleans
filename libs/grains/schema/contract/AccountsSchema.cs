using System.Text.Json;
using System.Threading.Tasks;
using Json.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace grains.schema.contract;

public class AccountsSchema
{
  JsonSchema Schema = JsonSchema.FromText("""
                                           {
                                            "$schema": "http://json-schema.org/draft-07/schema#",
                                            "properties": {
                                              "profile": {
                                                "properties": {
                                                  "firstName": {
                                                    "type": "string"
                                                  },
                                                  "lastName": {
                                                    "type": "string"
                                                  }
                                                }
                                              },
                                              "data": {
                                                "properties": {
                                                  "campaign": {
                                                    "type": "string"
                                                  },
                                                  "customer-id": {
                                                    "type": "string"
                                                  }
                                                }
                                              }
                                            }
                                          }
                                          """);

  public Task<SchemaValidationResult> ValidateAsync(JsonElement instance)
  {
    return Task.FromResult(new SchemaValidationResult(this.Schema.Evaluate(instance)));
  }
}
