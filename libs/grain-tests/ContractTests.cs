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


    /*response
      {
         "state": "connect",
         "authentication_token": "https://jwt.io/#debugger-io?token=eyJhbGciOiJFUzI1NiIsInN1YiI6IkpvbkBtYWlsLmNvbSIsImRjIjoidXMxIiwidGlkIjoidXJuOmdpZ3lhOnRpZDp0bnQtYW1zb3M6OTA5OjEyMTEyMzIzIiwiY2lkIjoiPGNvbnRleHQtaWQgKHJhbmRvbSBjb25zaXN0IGJldHdlZW4gdHdvIHRva2Vucyk-IiwianRpIjoxMzMxOTE5MDk4LCJ0eXAiOiJpZCtqd3MifQ.eyJhbXIiOiJndWVzdCIsImFjciI6InVybjpnaWd5YTpndWVzdCIsInVzZXIiOnsiZm9ybWF0IjoiZW1haWwiLCJkYyI6InVzMSIsImlkIjoiSm9uQG1haWwuY29tIn0sInRlbmFudCI6eyJzaXRlLWlkIjoxMjExMjMyMywiZ3JvdXAtaWQiOjEzMjEzMjMyMSwicGFydG5lci1pZCI6OTA5LCJ0ZW5hbnQtaWQiOiJ0bnQtYW1zb3MifSwiZGV2aWNlIjp7IklkIjoiZGV2aWNlLTEyMyIsIklwIjoiMTkyLjE2OC45MDEuwqciLCJVc2VyQWdlbnQiOiJNb3ppbGxhLzUuMCAoV2luZG93cyBOVCAxMC4wOyBXaW42NDsgeDY0KSBBcHBsZVdlYktpdC81MzcuMzYifSwiYXV0aCI6eyJtZXRob2QiOiJndWVzdCIsImNoYW5uZWwiOiJpZGVudGlmaWVyIn0sImlzcyI6Imh0dHBzOi8vb2F1dGgyLmdpZ3lhLmNvbSIsImV4cCI6MTcwMjk3NzU1NCwiaWF0IjoxNzAyOTc3MjU0LCJuYmYiOjE3MDI5NzcyNTR9.TXxor_zmFP8eeanAfoAzOwjhngQyeUkR97kA1n9mx5mvr9EHZpwZ-myIgiVBeZiFD3qMz713Tyq_l8ioan6uHw",
         "connect_token": "https://jwt.io/#debugger-io?token=eyJhbGciOiJFUzI1NiIsInN1YiI6IkpvbkBtYWlsLmNvbSIsImRjIjoidXMxIiwidGlkIjoidXJuOmdpZ3lhOnRpZDp0bnQtYW1zb3M6OTA5OjEyMTEyMzIzIiwiY2lkIjoiPGNvbnRleHQtaWQgKHJhbmRvbSBjb25zaXN0IGJldHdlZW4gdHdvIHRva2Vucyk-IiwianRpIjoxNDk4NzU3MDA5LCJ0eXAiOiJjb25uZWN0K2p3cyJ9.eyJjb250ZXh0Ijp7InVzZXIiOnsiZm9ybWF0IjoiZW1haWwiLCJkYyI6InVzMSIsImlkIjoiSm9uQG1haWwuY29tIn0sInRlbmFudCI6eyJzaXRlLWlkIjoxMjExMjMyMywiZ3JvdXAtaWQiOjEzMjEzMjMyMSwicGFydG5lci1pZCI6OTA5LCJ0ZW5hbnQtaWQiOiJ0bnQtYW1zb3MifSwiZGV2aWNlIjp7IklkIjoiZGV2aWNlLTEyMyIsIklwIjoiMTkyLjE2OC45MDEuwqciLCJVc2VyQWdlbnQiOiJNb3ppbGxhLzUuMCAoV2luZG93cyBOVCAxMC4wOyBXaW42NDsgeDY0KSBBcHBsZVdlYktpdC81MzcuMzYifX0sInN0YXRlIjoiY29ubmVjdCIsInN0YXRlcyI6eyJjaGFsbGVuZ2UiOnsidHlwZSI6ImlkZW50aWZpY2F0aW9uIiwiYWNyX3ZhbHVlcyI6InVybjpnaWd5YTpndXN0IiwidHlwZSI6ImlkZW50aWZpY2F0aW9uIiwiaGFuZGxlZCI6IjIwMjMtMTItMTlUMTE6MTQ6MTQuMDk5MDg4KzAyOjAwIn0sImNvbm5lY3QiOnsidXJuOmFjY291bnQ6dXBkYXRlIjp7InR5cGUiOiJ1cm46YWNjb3VudDp1cGRhdGUiLCJjb21tdW5pY2F0aW9uIjp7Im5ld3MiOnsic3RhdHVzIjoib3B0LWluIn19LCJwcmVmZXJlbmNlcyI6eyJ0b3MiOnsiaXNDb25zZW50R3JhbnRlZCI6dHJ1ZX19fSwidXJuOmFjY291bnQ6dXBkYXRlOmNyZWRlbnRpYWwiOnsidHlwZSI6InVybjphY2NvdW50OnVwZGF0ZTpjcmVkZW50aWFsIiwiZm9ybWF0IjoiZW1haWwiLCJ2YWx1ZSI6IkpvbkBtYWlsLmNvbSJ9LCJ1cm46YWNjb3VudDp1cGRhdGU6aWRlbnRpdHkiOnsidHlwZSI6InVybjphY2NvdW50OnVwZGF0ZTppZGVudGl0eSIsInByb3ZpZGVyIjoic2l0ZSIsImlkIjpudWxsLCJwcm9maWxlIjp7ImZpcnN0TmFtZSI6IkpvbiIsImxhc3ROYW1lIjoiU21pdGgiLCJlbWFpbCI6ImpvbmR1ZUBtYWlsLmNvbSJ9LCJkYXRhIjp7ImNhbXBhaWduIjoiam9uQGZvIiwiY3VzdG9tZXItaWQiOiIxMjEyMTMifX19fSwiYXVkIjoiaHR0cHM6Ly9hY2NvdW50cy51czEuZ2lneWEuY29tL2lkZW50aXR5LmNvbm5lY3QiLCJpc3MiOiJodHRwczovL29hdXRoMi5naWd5YS5jb20iLCJleHAiOjE3MDI5Nzc1NTQsImlhdCI6MTcwMjk3NzI1NCwibmJmIjoxNzAyOTc3MjU0fQ.hp_tn7_0f4J5rS7bLe2thm_Tv3SOtB587q3Wea7lsEDoi3D1yCPWw64Ls_HLJNid0e2Qc9Jf7kDYMbnlHkGATQ"
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
         "state": "connect",
         "states": {
           "challenge": {
             "type": "identification",
             "acr_values": "urn:gigya:gust",
             "handled": "2023-12-19T11:14:14.099088+02:00"
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
           }
         },
         "aud": "https://accounts.us1.gigya.com/identity.connect",
         "iss": "https://oauth2.gigya.com",
         "exp": 1702977554,
         "iat": 1702977254,
         "nbf": 1702977254
       }]
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
         "connect_token": "https://jwt.io/#debugger-io?token=eyJhbGciOiJFUzI1NiIsInN1YiI6IkpvbkBtYWlsLmNvbSIsImRjIjoidXMxIiwidGlkIjoidXJuOmdpZ3lhOnRpZDp0bnQtYW1zb3M6OTA5OjEyMTEyMzIzIiwiY2lkIjoiPGNvbnRleHQtaWQgKHJhbmRvbSBjb25zaXN0IGJldHdlZW4gdHdvIHRva2Vucyk-IiwianRpIjo2MzU4MzU1MTQsInR5cCI6ImNvbm5lY3QrandzIn0.eyJjb250ZXh0Ijp7InVzZXIiOnsiZm9ybWF0IjoiZW1haWwiLCJkYyI6InVzMSIsImlkIjoiSm9uQG1haWwuY29tIn0sInRlbmFudCI6eyJzaXRlLWlkIjoxMjExMjMyMywiZ3JvdXAtaWQiOjEzMjEzMjMyMSwicGFydG5lci1pZCI6OTA5LCJ0ZW5hbnQtaWQiOiJ0bnQtYW1zb3MifSwiZGV2aWNlIjp7IklkIjoiZGV2aWNlLTEyMyIsIklwIjoiMTkyLjE2OC45MDEuwqciLCJVc2VyQWdlbnQiOiJNb3ppbGxhLzUuMCAoV2luZG93cyBOVCAxMC4wOyBXaW42NDsgeDY0KSBBcHBsZVdlYktpdC81MzcuMzYifX0sInN0YXRlIjoiY2hhbGxlbmdlIiwic3RhdGVzIjp7ImNoYWxsZW5nZSI6eyJ0eXBlIjoiYXV0aGVudGljYXRpb24iLCJhY3JfdmFsdWVzIjoidXJuOmdpZ3lhOmxvYTEwIiwidHlwZSI6ImF1dGhlbnRpY2F0aW9uIiwicHJvbXB0IjoidmVyaWZ5IiwidXNlciI6eyJmb3JtYXQiOiJlbWFpbCIsImRjIjoidXMxIiwiaWQiOiJKb25AbWFpbC5jb20ifX0sImNvbm5lY3QiOnsidXJuOmFjY291bnQ6dXBkYXRlIjp7InR5cGUiOiJ1cm46YWNjb3VudDp1cGRhdGUiLCJjb21tdW5pY2F0aW9uIjp7Im5ld3MiOnsic3RhdHVzIjoib3B0LWluIn19LCJwcmVmZXJlbmNlcyI6eyJ0b3MiOnsiaXNDb25zZW50R3JhbnRlZCI6dHJ1ZX19fSwidXJuOmFjY291bnQ6dXBkYXRlOmNyZWRlbnRpYWwiOnsidHlwZSI6InVybjphY2NvdW50OnVwZGF0ZTpjcmVkZW50aWFsIiwiZm9ybWF0IjoiZW1haWwiLCJ2YWx1ZSI6IkpvbkBtYWlsLmNvbSJ9LCJ1cm46YWNjb3VudDp1cGRhdGU6aWRlbnRpdHkiOnsidHlwZSI6InVybjphY2NvdW50OnVwZGF0ZTppZGVudGl0eSIsInByb3ZpZGVyIjoic2l0ZSIsImlkIjpudWxsLCJwcm9maWxlIjp7ImZpcnN0TmFtZSI6IkpvbiIsImxhc3ROYW1lIjoiU21pdGgiLCJlbWFpbCI6ImpvbmR1ZUBtYWlsLmNvbSJ9LCJkYXRhIjp7ImNhbXBhaWduIjoiam9uQGZvIiwiY3VzdG9tZXItaWQiOiIxMjEyMTMifX19fSwiYXVkIjoiaHR0cHM6Ly9hY2NvdW50cy51czEuZ2lneWEuY29tL2lkZW50aXR5LmNvbm5lY3QiLCJpc3MiOiJodHRwczovL29hdXRoMi5naWd5YS5jb20iLCJleHAiOjE3MDI5NzcwNzMsImlhdCI6MTcwMjk3Njc3MywibmJmIjoxNzAyOTc2NzczfQ.HprMHTlTNIcTvVVRzAl0hEnVOCHjzR_8vtJKjdhwblaoSfRgSxxzLutP5NJVsfRrj43ZxNMi02RAMgQ520t5jw"
       }
      }
    */



    /*  connect_token
       {
         "alg": "ES256",
         "sub": "Jon@mail.com",
         "dc": "us1",
         "tid": "urn:gigya:tid:tnt-amsos:909:12112323",
         "cid": "<context-id (random consist between two tokens)>",
         "jti": 635835514,
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
         "states": {
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
           }
         },
         "aud": "https://accounts.us1.gigya.com/identity.connect",
         "iss": "https://oauth2.gigya.com",
         "exp": 1702977073,
         "iat": 1702976773,
         "nbf": 1702976773
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
