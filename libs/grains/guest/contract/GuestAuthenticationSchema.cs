using System.Text.Json;
using System.Threading.Tasks;
using grains.schema.contract;
using Json.Schema;

namespace grains.guest.contract;

public class GuestAuthenticationSchema
{
  readonly JsonSchema _schema = JsonSchema.FromText("""
                                                    {
                                                      "$schema": "http://json-schema.org/draft-07/schema#",
                                                      "properties": {
                                                        "id": {
                                                          "type": "object",
                                                          "properties": {
                                                            "id": {
                                                              "type": "string"
                                                            },
                                                            "format": {
                                                              "type": "string"
                                                            }
                                                          }
                                                        },
                                                        "profile": {
                                                          "type": "object",
                                                          "properties": {
                                                            "firstName": true,
                                                            "lastName": true
                                                          },
                                                          "additionalProperties": false
                                                        },
                                                        "data": {
                                                          "type": "object",
                                                          "properties": {
                                                            "campaign": true
                                                          },
                                                          "additionalProperties": false
                                                        }
                                                      }
                                                    }

                                                    """);


  public Task<SchemaValidationResult> ValidateAsync(JsonElement instance)
  {
    return Task.FromResult(new SchemaValidationResult(this._schema.Evaluate(instance)));
  }


}
