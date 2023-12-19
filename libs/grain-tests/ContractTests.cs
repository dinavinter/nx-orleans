using System.Collections.Generic;
using System.Text.Json;
using grains.guest.contract;
using Xunit;
using Xunit.Abstractions;

namespace Test.SchemaLand.Api;

public class ContractTests
{
  private readonly ITestOutputHelper _output;

  public ContractTests(ITestOutputHelper output)
  {
    _output = output;
  }

  [Fact]
  public  void AuthenticationConnectState_Format()
  {
    var result = new AuthenticationConnectState( new GuestAuthenticationProperties(SampleRequest()));

    _output.WriteLine(JsonSerializer.Serialize(result));
  }

  [Fact]
  public void AuthorizationChallengeState_Format()
  {
    var result = new AuthenticationChallengeState( new GuestAuthenticationProperties(SampleRequest()));

    _output.WriteLine(JsonSerializer.Serialize(result));
  }

  private static GuestAuthenticationRequest SampleRequest()
  {
    return JsonSerializer.Deserialize<GuestAuthenticationRequest>("""
                                                                    {
                                                                      "id": {
                                                                        "format": "email",
                                                                        "value": "Jon@mail.com"
                                                                      },
                                                                      "profile": {
                                                                        "firstName": "Jon",
                                                                        "lastName": "Smith",
                                                                        "email": "jondue@mail.com"
                                                                      },
                                                                      "data": {
                                                                        "campaign": "jon@fo",
                                                                        "customer-id": "121213"
                                                                      },
                                                                        "communication": {
                                                                          "news": {
                                                                            "status": "opt-in"
                                                                          }
                                                                        },
                                                                        "preferences": {
                                                                          "tos": {
                                                                            "isConsentGranted": true
                                                                          }
                                                                      }
                                                                    }
                                                                  """
                                                                  );

  }
}
