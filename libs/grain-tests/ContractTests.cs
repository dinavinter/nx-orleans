using System.Collections.Generic;
using System.Text.Json;
using grains.guest.contract;
using grains.guest.contract.result;
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


    /*response
      {
         "state": "connect",
         "authentication_token": "https://jwt.io/#debugger-io?token=eyJhbGciOiJFUzI1NiIsInN1YiI6IkpvbkBtYWlsLmNvbSIsImRjIjoidXMxIiwidGlkIjoidXJuOmdpZ3lhOnRpZDp0bnQtYW1zb3M6OTA5OjEyMTEyMzIzIiwiY2lkIjoiPGNvbnRleHQtaWQgKHJhbmRvbSBjb25zaXN0IGJldHdlZW4gdHdvIHRva2Vucyk-IiwianRpIjoxMzMxOTE5MDk4LCJ0eXAiOiJpZCtqd3MifQ.eyJhbXIiOiJndWVzdCIsImFjciI6InVybjpnaWd5YTpndWVzdCIsInVzZXIiOnsiZm9ybWF0IjoiZW1haWwiLCJkYyI6InVzMSIsImlkIjoiSm9uQG1haWwuY29tIn0sInRlbmFudCI6eyJzaXRlLWlkIjoxMjExMjMyMywiZ3JvdXAtaWQiOjEzMjEzMjMyMSwicGFydG5lci1pZCI6OTA5LCJ0ZW5hbnQtaWQiOiJ0bnQtYW1zb3MifSwiZGV2aWNlIjp7IklkIjoiZGV2aWNlLTEyMyIsIklwIjoiMTkyLjE2OC45MDEuwqciLCJVc2VyQWdlbnQiOiJNb3ppbGxhLzUuMCAoV2luZG93cyBOVCAxMC4wOyBXaW42NDsgeDY0KSBBcHBsZVdlYktpdC81MzcuMzYifSwiYXV0aCI6eyJtZXRob2QiOiJndWVzdCIsImNoYW5uZWwiOiJpZGVudGlmaWVyIn0sImlzcyI6Imh0dHBzOi8vb2F1dGgyLmdpZ3lhLmNvbSIsImV4cCI6MTcwMjk3NzU1NCwiaWF0IjoxNzAyOTc3MjU0LCJuYmYiOjE3MDI5NzcyNTR9.TXxor_zmFP8eeanAfoAzOwjhngQyeUkR97kA1n9mx5mvr9EHZpwZ-myIgiVBeZiFD3qMz713Tyq_l8ioan6uHw",
         "connect_token": "https://jwt.io/#debugger-io?token=eyJhbGciOiJFUzI1NiIsInN1YiI6IkpvbkBtYWlsLmNvbSIsImRjIjoidXMxIiwidGlkIjoidXJuOmdpZ3lhOnRpZDp0bnQtYW1zb3M6OTA5OjEyMTEyMzIzIiwiY2lkIjoiPGNvbnRleHQtaWQgKHJhbmRvbSBjb25zaXN0IGJldHdlZW4gdHdvIHRva2Vucyk-IiwianRpIjoxODcxNzk5NzQwLCJ0eXAiOiJjb25uZWN0K2p3cyJ9.eyJjb250ZXh0Ijp7InVzZXIiOnsiZm9ybWF0IjoiZW1haWwiLCJkYyI6InVzMSIsImlkIjoiSm9uQG1haWwuY29tIn0sInRlbmFudCI6eyJzaXRlLWlkIjoxMjExMjMyMywiZ3JvdXAtaWQiOjEzMjEzMjMyMSwicGFydG5lci1pZCI6OTA5LCJ0ZW5hbnQtaWQiOiJ0bnQtYW1zb3MifSwiZGV2aWNlIjp7IklkIjoiZGV2aWNlLTEyMyIsIklwIjoiMTkyLjE2OC45MDEuwqciLCJVc2VyQWdlbnQiOiJNb3ppbGxhLzUuMCAoV2luZG93cyBOVCAxMC4wOyBXaW42NDsgeDY0KSBBcHBsZVdlYktpdC81MzcuMzYifX0sInN0YXRlIjoiY29ubmVjdCIsImNoYWxsZW5nZSI6eyJ0eXBlIjoiaWRlbnRpZmljYXRpb24iLCJhY3JfdmFsdWVzIjoidXJuOmdpZ3lhOmd1ZXN0IiwidHlwZSI6ImlkZW50aWZpY2F0aW9uIiwiaGFuZGxlZCI6IjIwMjMtMTItMTlUMTI6MzQ6NDIuODMyMTU2KzAyOjAwIn0sImNvbm5lY3QiOnsidXJuOmFjY291bnQ6dXBkYXRlIjp7InR5cGUiOiJ1cm46YWNjb3VudDp1cGRhdGUiLCJjb21tdW5pY2F0aW9uIjp7Im5ld3MiOnsic3RhdHVzIjoib3B0LWluIn19LCJwcmVmZXJlbmNlcyI6eyJ0b3MiOnsiaXNDb25zZW50R3JhbnRlZCI6dHJ1ZX19fSwidXJuOmFjY291bnQ6dXBkYXRlOmNyZWRlbnRpYWwiOnsidHlwZSI6InVybjphY2NvdW50OnVwZGF0ZTpjcmVkZW50aWFsIiwiZm9ybWF0IjoiZW1haWwiLCJ2YWx1ZSI6IkpvbkBtYWlsLmNvbSJ9LCJ1cm46YWNjb3VudDp1cGRhdGU6aWRlbnRpdHkiOnsidHlwZSI6InVybjphY2NvdW50OnVwZGF0ZTppZGVudGl0eSIsInByb3ZpZGVyIjoic2l0ZSIsImlkIjpudWxsLCJwcm9maWxlIjp7ImZpcnN0TmFtZSI6IkpvbiIsImxhc3ROYW1lIjoiU21pdGgiLCJlbWFpbCI6ImpvbmR1ZUBtYWlsLmNvbSJ9LCJkYXRhIjp7ImNhbXBhaWduIjoiam9uQGZvIiwiY3VzdG9tZXItaWQiOiIxMjEyMTMifX19LCJhdWQiOiJodHRwczovL2FjY291bnRzLnVzMS5naWd5YS5jb20vaWRlbnRpdHkuY29ubmVjdCIsImlzcyI6Imh0dHBzOi8vb2F1dGgyLmdpZ3lhLmNvbSIsImV4cCI6MTcwMjk4MjM4MiwiaWF0IjoxNzAyOTgyMDgyLCJuYmYiOjE3MDI5ODIwODJ9.zWsJFfOMVcpYmSbqvCkA5gAKpO-_QluLnNrBbRo0Qdjx8xAzXKtIUEiBG31oX8BpYX0AxUPbsg8uTCqcUSs-Mg"
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
           }
         },
         "state": "challenge",
         "challenge": {
           "type": "authentication",
           "acr_values": "urn:gigya:loa10",
           "prompt": "verify"
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
             }
           },
           "urn:account:update:credential": {
             "type": "urn:account:update:credential",
             "format": "email",
             "value": "Jon@mail.com"
           },
           "urn:account:update:identity": {
             "type": "urn:account:update:identity",
             "provider": "site",
             "id": null,
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
         },
         "aud": "https://accounts.us1.gigya.com/identity.connect",
         "iss": "https://oauth2.gigya.com",
         "exp": 1702982382,
         "iat": 1702982082,
         "nbf": 1702982082
       }
     */

    /* authentication token
     {
         "alg": "ES256",
         "sub": "Jon@mail.com",
         "dc": "us1",
         "tid": "urn:gigya:tid:tnt-amsos:909:12112323",
         "cid": "<context-id (random consist between two tokens)>",
         "jti": 1331919098,
         "typ": "id+jws"
     },
     {
         "amr": "guest",
         "acr": "urn:gigya:guest",
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
         },
         "iss": "https://oauth2.gigya.com",
         "exp": 1702977554,
         "iat": 1702977254,
         "nbf": 1702977254
       }
     */
  }

  [Fact]
  public void AuthorizationChallengeState_Format()
  {
    var result = new AuthenticationChallengeState( new GuestAuthenticationProperties(SampleRequest()));

    _output.WriteLine(JsonSerializer.Serialize(result));

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



    /*  connect_token
       {
         "context": {
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
           }
         },
         "state": "connect",
         "challenge": {
           "type": "identification",
           "acr_values": "urn:gigya:guest",
           "handled": "2023-12-19T12:34:42.832156+02:00"
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
             }
           },
           "urn:account:update:credential": {
             "type": "urn:account:update:credential",
             "format": "email",
             "value": "Jon@mail.com"
           },
           "urn:account:update:identity": {
             "type": "urn:account:update:identity",
             "provider": "site",
             "id": null,
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
         },
         "aud": "https://accounts.us1.gigya.com/identity.connect",
         "iss": "https://oauth2.gigya.com",
         "exp": 1702982382,
         "iat": 1702982082,
         "nbf": 1702982082
       }
      */

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
