using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using System.Text.Json.Nodes;
using grains.guest.contract;
using grains.guest.contract.result;
using Microsoft.IdentityModel.JsonWebTokens;
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


  //// <example title="connect token payload">
  /* {
     "context": {
       "tenant": {
         "site-id": 12112323,
         "group-id": 132132321,
         "partner-id": 909,
         "tenant-id": "tnt-amsos"
       },
       "device": {
         "Id": "device-123",
         "Ip": "192.168.901.§",
         "UserAgent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36"
       }
     },
     "challenge": {
       "type": "authentication",
       "acr_values": "urn:gigya:guest",
       "prompt": "verify",
       "user": {
         "format": "email",
         "dc": "us1",
         "id": "Jon@mail.com"
       }
     },
     "connect": {
       "urn:account:update": {
         "type": "urn:account:update",
         "communication": {
           "news": {
             "status": "opt-in"
           }
         },
         "preferences": {
           "tos": {
             "isConsentGranted": true
           }
         },
         "profile": {
           "firstName": "Jon",
           "lastName": "Smith",
           "email": "Jon@mail.com"
         }
       },
       "urn:account:update:credential": {
         "type": "urn:account:update:credential",
         "format": "email",
         "value": "Jon@mail.com"
       }
     }
   }
   */
  /// </example>
  /// <example title="authentication token payload">
 /*
    {
      "acr": "urn:gigya:guest",
      "amr": "guest",
      "user": {
        "format": "email",
        "dc": "us1",
        "id": "Jon@mail.com"
        },
      "tenant": {
        "site-id": 12112323,
        "group-id": 132132321,
        "partner-id": 909,
        "tenant-id": "tnt-amsos"
        },
       "device": {
         "Id": "device-123",
         "Ip": "192.168.901.§",
         "UserAgent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36"
        },
      "auth": {
        "method": "guest",
        "channel": "identifier"
      }
    }

  */
 ///</example>
  /// <summary>
  /// test response contract for guest authentication, where the guest authentication is sufficient
  /// </summary>
  /// <returns> json response contains both connect token and authentication token</returns>
  [Fact]
  public  void AuthenticationConnectState_Format()
  {
        var request = JsonSerializer.Deserialize<GuestAuthenticationRequest>("""
                                                                    {
                                                                      "siteId": 12112323,
                                                                      "groupId": 132132321,
                                                                      "partnerId": 909,
                                                                      "tenantId": "tnt-amsos",
                                                                      "deviceId": "device-123",
                                                                      "ip": "192.168.901.§",
                                                                      "userAgent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36",

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


    var result = new AuthenticationConnectState( new GuestAuthenticationProperties( request));

    _output.WriteLine(JsonSerializer.Serialize(result));

    var connectToken = new JsonWebTokenHandler().ReadJsonWebToken(result.ConnectToken.Token);

    // asert connect token headers
    Assert.Equal("connect+jws", connectToken.Typ);
    Assert.Equal("https://oauth2.gigya.com", connectToken.Issuer);
    Assert.Equivalent(new []{"https://accounts.us1.gigya.com/identity.connect"}, connectToken.Audiences);
    Assert.Equal("ES256", connectToken.Alg);
    Assert.Equal("urn:gigya:tid:tnt-amsos:909:12112323",  connectToken.GetHeaderValue<string>("tid") );
    Assert.NotNull(connectToken.GetHeaderValue<string>("cid"));
    Assert.NotNull(connectToken.GetHeaderValue<string>("jti"));


    // assert connect token payload
     Assert.Equal("connect", connectToken.GetPayloadValue<string>("state"));

    Assert.Equivalent(JsonDocument.Parse("""
      {
                      "tenant": {
                        "site-id": 12112323,
                        "group-id": 132132321,
                        "partner-id": 909,
                        "tenant-id": "tnt-amsos"
                      },
                      "device": {
                        "Id": "device-123",
                        "Ip": "192.168.901.§",
                        "UserAgent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36"
                      }
                    }
      """).RootElement, connectToken.GetPayloadValue<JsonElement>("context"));

    Assert.Equivalent(JsonDocument.Parse("""
                                               {
                                                         "type": "authentication",
                                                         "acr_values": "urn:gigya:guest",
                                                         "user": {
                                                           "format": "email",
                                                           "dc": "us1",
                                                            "id": "Jon@mail.com"
                                                            }
                                                }
                                         """).RootElement, connectToken.GetPayloadValue<JsonElement>("challenge"));


    Assert.Equivalent(JsonDocument.Parse("""
                                         {
                                           "format": "email",
                                           "value": "Jon@mail.com"
                                         }

                                         """).RootElement, connectToken.GetPayloadValue<JsonElement>("connect").GetProperty("urn:account:update:credential"));


    Assert.Equivalent(JsonDocument.Parse("""
                                         {
                                           "communication": {
                                             "news": {
                                               "status": "opt-in"
                                             }
                                           },
                                           "preferences": {
                                             "tos": {
                                               "isConsentGranted": true
                                             }
                                           },
                                           "profile": {
                                                       "firstName": "Jon",
                                                       "lastName": "Smith",
                                                       "email": "jondue@mail.com"
                                                     },
                                                     "data": {
                                                       "campaign": "jon@fo",
                                                       "customer-id": "121213"
                                                     }
                                          }

                                         """).RootElement,
      connectToken.GetPayloadValue<JsonElement>("connect").GetProperty("urn:account:update"));

    Assert.Equivalent(JsonDocument.Parse("""
                                         {
                                          "provider": "guest",
                                          "id": "<guest-id>",
                                          "profile": {
                                            "firstName": "Jon",
                                            "lastName": "Smith",
                                            "email": "Jon@mail.com"
                                            },
                                             "data": {
                                               "campaign": "jon@fo",
                                               "customer-id": "121213"
                                             }
                                          }

                                         """).RootElement,
      connectToken.GetPayloadValue<JsonElement>("connect").GetProperty("urn:account:update:identity"));


    var authenticationToken =new JsonWebTokenHandler().ReadJsonWebToken(result.AuthToken.Token);
      // new JsonWebTokenHandler().ReadJsonWebToken(JsonDocument
      // .Parse(JsonSerializer.Serialize(result))
      // .RootElement.GetProperty("authentication_token").GetString());

    //assert authentication token headers
    Assert.Equal("id+jws", authenticationToken.Typ);
    Assert.Equal("https://oauth2.gigya.com", connectToken.Issuer);
    Assert.Equal("ES256", connectToken.Alg);
    Assert.Equal("urn:gigya:tid:tnt-amsos:909:12112323",  connectToken.GetHeaderValue<string>("tid") );
    Assert.NotNull(connectToken.GetHeaderValue<string>("cid"));
    Assert.NotNull(connectToken.GetHeaderValue<string>("jti"));

    //assert authentication token payload
    Assert.Equal("guest", authenticationToken.GetPayloadValue<string>("amr"));

    Assert.Equivalent(JsonDocument.Parse("""
                                        {
                                             "format": "email",
                                             "dc": "us1",
                                             "id": "Jon@mail.com"
                                      }
                                      """).RootElement, authenticationToken.GetPayloadValue<JsonElement>("user"));

    Assert.Equivalent(JsonDocument.Parse("""
                                           {
                                                "site-id": 12112323,
                                                "group-id": 132132321,
                                                "partner-id": 909,
                                                "tenant-id": "tnt-amsos"
                                         }
                                         """).RootElement, authenticationToken.GetPayloadValue<JsonElement>("tenant"));

    Assert.Equivalent(JsonDocument.Parse("""
                                           {
                                                "Id": "device-123",
                                                "Ip": "192.168.901.§",
                                                "UserAgent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36"
                                         }
                                         """).RootElement, authenticationToken.GetPayloadValue<JsonElement>("device"));


    Assert.Equivalent(JsonDocument.Parse("""
                                           {
                                                "method": "guest",
                                                "channel": "identifier"
                                         }
                                         """).RootElement, authenticationToken.GetPayloadValue<JsonElement>("auth"));



  }

  [Fact]
  public void AuthorizationChallengeState_Format()
  {
    var request = JsonSerializer.Deserialize<GuestAuthenticationRequest>("""
                                                                    {
                                                                      "siteId": 12112323,
                                                                      "groupId": 132132321,
                                                                      "partnerId": 909,
                                                                      "tenantId": "tnt-amsos",
                                                                      "deviceId": "device-123",
                                                                      "ip": "192.168.901.§",
                                                                      "userAgent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36",

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

    var result = new AuthenticationChallengeState( new GuestAuthenticationProperties(request));

    _output.WriteLine(JsonSerializer.Serialize(result));
    var connectToken = new JsonWebTokenHandler().ReadJsonWebToken(result.ConnectToken.Token);
    Assert.Equal("connect+jws", connectToken.Typ);
    Assert.Equal("https://oauth2.gigya.com", connectToken.Issuer);
    Assert.Equivalent(new []{"https://accounts.us1.gigya.com/identity.connect"}, connectToken.Audiences);
    Assert.Equal("ES256", connectToken.Alg);
    Assert.Equal("urn:gigya:tid:tnt-amsos:909:12112323",  connectToken.GetHeaderValue<string>("tid") );
    Assert.NotNull(connectToken.GetHeaderValue<string>("cid"));
    Assert.NotNull(connectToken.GetHeaderValue<string>("jti"));
     Assert.Equal("challenge", connectToken.GetPayloadValue<string>("state"));
    Assert.Equivalent(JsonDocument.Parse("""
                                         {
                                                         "tenant": {
                                                           "site-id": 12112323,
                                                           "group-id": 132132321,
                                                           "partner-id": 909,
                                                           "tenant-id": "tnt-amsos"
                                                         },
                                                         "device": {
                                                           "Id": "device-123",
                                                           "Ip": "192.168.901.§",
                                                           "UserAgent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36"
                                                         }
                                                       }
                                         """).RootElement,
      connectToken.GetPayloadValue<JsonElement>("context"));

    Assert.Equivalent(JsonDocument.Parse("""
                                               {
                                                         "type": "authentication",
                                                         "acr_values": "urn:gigya:loa10",
                                                         "prompt": "verify",
                                                         "user": {
                                                           "format": "email",
                                                           "dc": "us1",
                                                            "id": "Jon@mail.com"
                                                            }
                                                }
                                         """).RootElement,
      connectToken.GetPayloadValue<JsonElement>("challenge"));


     Assert.Equivalent(JsonDocument.Parse("""
                                         {
                                           "format": "email",
                                           "value": "Jon@mail.com"
                                         }

                                         """).RootElement,
       connectToken.GetPayloadValue<JsonElement>("connect").GetProperty("urn:account:update:credential"));


    Assert.Equivalent(JsonDocument.Parse("""
                                         {
                                           "communication": {
                                             "news": {
                                               "status": "opt-in"
                                             }
                                           },
                                           "preferences": {
                                             "tos": {
                                               "isConsentGranted": true
                                             }
                                           },
                                           "profile": {
                                                       "firstName": "Jon",
                                                       "lastName": "Smith",
                                                       "email": "jondue@mail.com"
                                                     },
                                                     "data": {
                                                       "campaign": "jon@fo",
                                                       "customer-id": "121213"
                                                     }
                                          }

                                         """).RootElement,
      connectToken.GetPayloadValue<JsonElement>("connect").GetProperty("urn:account:update"));

    Assert.Equivalent(JsonDocument.Parse("""
                                         {
                                          "provider": "guest",
                                          "id": "<guest-id>",
                                          "profile": {
                                            "firstName": "Jon",
                                            "lastName": "Smith",
                                            "email": "Jon@mail.com"
                                            },
                                             "data": {
                                               "campaign": "jon@fo",
                                               "customer-id": "121213"
                                             }
                                          }

                                         """).RootElement,
      connectToken.GetPayloadValue<JsonElement>("connect").GetProperty("urn:account:update:identity"));



      /* response
      {
         "state": "challenge",
         "challenge": {
           "type": "authentication",
           "acr_values": "urn:gigya:loa10",
           "type": "authentication",
           "prompt": "verify",
           "user": {
             "format": "email",
             "dc": "us1",
             "id": "Jon@mail.com"
           }
         },
         "connect_token": "https://jwt.io/#debugger-io?token=eyJhbGciOiJFUzI1NiIsInN1YiI6IkpvbkBtYWlsLmNvbSIsImRjIjoidXMxIiwidGlkIjoidXJuOmdpZ3lhOnRpZDp0bnQtYW1zb3M6OTA5OjEyMTEyMzIzIiwiY2lkIjoiPGNvbnRleHQtaWQgKHJhbmRvbSBjb25zaXN0IGJldHdlZW4gdHdvIHRva2Vucyk-IiwianRpIjoxNDM3NDYzODUyLCJ0eXAiOiJjb25uZWN0K2p3cyJ9.eyJjb250ZXh0Ijp7InVzZXIiOnsiZm9ybWF0IjoiZW1haWwiLCJkYyI6InVzMSIsImlkIjoiSm9uQG1haWwuY29tIn0sInRlbmFudCI6eyJzaXRlLWlkIjoxMjExMjMyMywiZ3JvdXAtaWQiOjEzMjEzMjMyMSwicGFydG5lci1pZCI6OTA5LCJ0ZW5hbnQtaWQiOiJ0bnQtYW1zb3MifSwiZGV2aWNlIjp7IklkIjoiZGV2aWNlLTEyMyIsIklwIjoiMTkyLjE2OC45MDEuwqciLCJVc2VyQWdlbnQiOiJNb3ppbGxhLzUuMCAoV2luZG93cyBOVCAxMC4wOyBXaW42NDsgeDY0KSBBcHBsZVdlYktpdC81MzcuMzYifX0sInN0YXRlIjoiY2hhbGxlbmdlIiwiY2hhbGxlbmdlIjp7InR5cGUiOiJhdXRoZW50aWNhdGlvbiIsImFjcl92YWx1ZXMiOiJ1cm46Z2lneWE6bG9hMTAiLCJ0eXBlIjoiYXV0aGVudGljYXRpb24iLCJwcm9tcHQiOiJ2ZXJpZnkifSwiY29ubmVjdCI6eyJ1cm46YWNjb3VudDp1cGRhdGUiOnsidHlwZSI6InVybjphY2NvdW50OnVwZGF0ZSIsImNvbW11bmljYXRpb24iOnsibmV3cyI6eyJzdGF0dXMiOiJvcHQtaW4ifX0sInByZWZlcmVuY2VzIjp7InRvcyI6eyJpc0NvbnNlbnRHcmFudGVkIjp0cnVlfX19LCJ1cm46YWNjb3VudDp1cGRhdGU6Y3JlZGVudGlhbCI6eyJ0eXBlIjoidXJuOmFjY291bnQ6dXBkYXRlOmNyZWRlbnRpYWwiLCJmb3JtYXQiOiJlbWFpbCIsInZhbHVlIjoiSm9uQG1haWwuY29tIn0sInVybjphY2NvdW50OnVwZGF0ZTppZGVudGl0eSI6eyJ0eXBlIjoidXJuOmFjY291bnQ6dXBkYXRlOmlkZW50aXR5IiwicHJvdmlkZXIiOiJzaXRlIiwiaWQiOm51bGwsInByb2ZpbGUiOnsiZmlyc3ROYW1lIjoiSm9uIiwibGFzdE5hbWUiOiJTbWl0aCIsImVtYWlsIjoiam9uZHVlQG1haWwuY29tIn0sImRhdGEiOnsiY2FtcGFpZ24iOiJqb25AZm8iLCJjdXN0b21lci1pZCI6IjEyMTIxMyJ9fX0sImF1ZCI6Imh0dHBzOi8vYWNjb3VudHMudXMxLmdpZ3lhLmNvbS9pZGVudGl0eS5jb25uZWN0IiwiaXNzIjoiaHR0cHM6Ly9vYXV0aDIuZ2lneWEuY29tIiwiZXhwIjoxNzAyOTgyMzgyLCJpYXQiOjE3MDI5ODIwODIsIm5iZiI6MTcwMjk4MjA4Mn0.X3ZDNUKQ793c495e_fgfwytD4FOfJBXm7g5ZOYuw7wu9aMnnzoMJ4UuyNnDLP4qCAWT5MvZVRvEnJ9izah5Rug"
       }
      }
    */
/* connect token
     [{
         "alg": "ES256",
         "sub": "Jon@mail.com",
         "dc": "us1",
         "tid": "urn:gigya:tid:tnt-amsos:909:12112323",
         "cid": "<context-id (random consist between two tokens)>",
         "jti": 1498757009,
         "typ": "connect+jws"
       },
       {
                "context": {
                  "tenant": {
                    "site-id": 12112323,
                    "group-id": 132132321,
                    "partner-id": 909,
                    "tenant-id": "tnt-amsos"
                  },
                  "device": {
                    "Id": "device-123",
                    "Ip": "192.168.901.§",
                    "UserAgent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36"
                  }
                },
                "challenge": {
                  "type": "authentication",
                  "acr_values": "urn:gigya:loa10",
                  "prompt": "verify",
             "user": {
              "format": "email",
              "dc": "us1",
                "id": "Jon@mail.com"
                  }
                },
                "connect": {
                  "urn:account:update": {
                    "type": "urn:account:update",
                    "communication": {
                      "news": {
                        "status": "opt-in"
                      }
                    },
                    "preferences": {
                      "tos": {
                        "isConsentGranted": true
                      }
                    },
             "profile": {
                      "firstName": "Jon",
                      "lastName": "Smith",
                      "email": "jondue@mail.com"
                    },
                    "data": {
                      "campaign": "jon@fo",
                      "customer-id": "121213"
                    }
                  },
                  "urn:account:update:credential": {
                    "type": "urn:account:update:credential",
                    "format": "email",
                    "value": "Jon@mail.com"
                  },
                  "urn:account:update:identity": {
                   "$comment": "no need for this phase of guest feature",
                    "type": "urn:account:update:identity",
                    "provider": "guest",
                    "id": "<guest-id>",
                    "profile": {
                      "firstName": "Jon",
                      "lastName": "Smith",
                      "email": "jondue@mail.com"
                    },
                    "data": {
                      "campaign": "jon@fo",
                      "customer-id": "121213"
                    }
                  }
                }
       },
         "aud": "https://accounts.us1.gigya.com/identity.connect",
         "iss": "https://oauth2.gigya.com",
         "exp": 1702982382,
         "iat": 1702982082,
         "nbf": 1702982082
       }
     */



  }

}
