using FsCheck.Xunit;
using Orleans.Contrib.UniversalSilo;
using Orleans.TestingHost;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using grains.guest.contract;
using Xunit;
using Xunit.Abstractions;

namespace Test.SchemaLand.Api
{
    /// <summary>
    /// This is needed to group tests together into a fixture
    /// </summary>
    [CollectionDefinition(Name)]
    public class ClusterCollection : ICollectionFixture<ClusterFixture>
    {
        public const string Name = nameof(ClusterCollection);
    }

    /// <summary>
    /// These are the tests grouped by fixture
    /// </summary>
    [Collection(ClusterCollection.Name)]
    public class GrainTests
    {
      private readonly ITestOutputHelper _output;
      private readonly TestCluster _cluster;
        private readonly IAuthGuest _guest;

        public GrainTests(ClusterFixture fixture, ITestOutputHelper output)
        {
          _output = output;
          // extract the TestCluster instance here and save it...
            _cluster = fixture?.Cluster ?? throw new ArgumentNullException(nameof(fixture));

            // create a single grain to test if you want here, or alternately create a grain in the test itself
            _guest = _cluster.GrainFactory.GetGrain<IAuthGuest>(Guid.NewGuid());
        }

        /// <summary>
        /// This is a traditional unit test.
        ///
        /// Provide known inputs and check the actual result against a known expected value
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task GuestAuth()
        {
            // this is an example of creating a grain within the test from using the TestCluster instance
            var authGuest = _cluster.GrainFactory.GetGrain<IAuthGuest>(Guid.NewGuid());

            var result = await authGuest.AuthenticateAsync(new GuestAuthenticationProperties(
              new GuestAuthenticationRequest()
              {
                  Id = new ()
                  {
                    Format = "email",
                    Value = "jon@smith.com"
                  },
                  Profile =  JsonDocument.Parse("""
                                                       {
                                                         "firstName": "Jon",
                                                         "lastName": "Smith"
                                                         }
                                                      """).RootElement,
                   Data = JsonDocument.Parse("""
                                                   {
                                                     "campaign": "jon@fo"
                                                   }
                                                  """).RootElement,

                  UserInfo = new Dictionary<string, JsonElement>()
                  {
                    ["communication"] = JsonDocument.Parse("""
                                                   {
                                                     "news":  { "status": "opt-in"  }
                                                   }
                                                  """).RootElement,
                    ["preferences"] = JsonDocument.Parse("""
                                                   {
                                                     "tos": {"isConsentGranted": true}
                                                   }
                                                  """).RootElement
                   }

              }));

            _output.WriteLine(JsonSerializer.Serialize(result));

        }

        [Fact]
        public async Task GuestAuth_Challange()
        {
            // this is an example of creating a grain within the test from using the TestCluster instance
            var authGuest = _cluster.GrainFactory.GetGrain<IAuthGuest>(Guid.NewGuid());
            var result = await authGuest.AuthenticateAsync(new GuestAuthenticationProperties(
              new GuestAuthenticationRequest()
              {
                  Id = new ()
                  {
                    Format = "email",
                    Value = "jon@smith.com"
                  },
                  Profile =  JsonDocument.Parse("""
                                                       {
                                                         "firstName": "Jon",
                                                         "lastName": "Smith",
                                                         "email": "jon@fo"
                                                       }
                                                      """).RootElement,
                   Data = JsonDocument.Parse("""
                                                   {
                                                     "campaign": "jon@fo",
                                                     "customer-id": "121213"
                                                   }
                                                  """).RootElement,

                  UserInfo = new Dictionary<string, JsonElement>()
                  {
                    ["communication"] = JsonDocument.Parse("""
                                                   {
                                                     "news":  { "status": "opt-in"  }
                                                   }
                                                  """).RootElement,
                    ["preferences"] = JsonDocument.Parse("""
                                                   {
                                                     "tos": {"isConsentGranted": true}
                                                   }
                                                  """).RootElement
                   }

              }));
            _output.WriteLine(JsonSerializer.Serialize(result));

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
          return new GuestAuthenticationRequest()
          {
            Id = new()
            {
              Format = "email",
              Value = "jon@smith.com"
            },
            Profile = JsonDocument.Parse("""
                                          {
                                            "firstName": "Jon",
                                            "lastName": "Smith",
                                            "email": "jon@fo"
                                          }
                                         """).RootElement,
            Data = JsonDocument.Parse("""
                                       {
                                         "campaign": "jon@fo",
                                         "customer-id": "121213"
                                       }
                                      """).RootElement,

            UserInfo = new Dictionary<string, JsonElement>()
            {
              ["communication"] = JsonDocument.Parse("""
                                                      {
                                                        "news":  { "status": "opt-in"  }
                                                      }
                                                     """).RootElement,
              ["preferences"] = JsonDocument.Parse("""
                                                    {
                                                      "tos": {"isConsentGranted": true}
                                                    }
                                                   """).RootElement
            }

          };
        }

        // /// <summary>
        // /// This is a property-based test.
        // ///
        // /// The test-runner will generate a large number of random values for
        // /// <paramref name="l"/> and <paramref name="r"/> each time the test is run,
        // /// and we will test a **property** of the addition operation.
        // ///
        // /// Mathematically, addition is the *only* operation to be
        // ///     * associative
        // ///     * commutative
        // ///     * have an identity of zero
        // /// so checking these three properties proves the addition operation is correct.
        // ///
        // /// In this test, we check that zero is the additive identity.
        // ///
        // /// </summary>
        // /// <param name="l"></param>
        // /// <param name="r"></param>
        // /// <returns>`true` if the test should pass, `false` otherwise</returns>
        // [Property]
        // public bool AdditionIdentityIsZero(int x)
        // {
        //     // Never call `.Result` on a grain call!
        //     // Use the trick of `await`ing in a local `async` function if possible!
        //     async Task<bool> identityCheck()
        //     {
        //         var forwardSum = await _calculatorGrain.Add(0, x);
        //         var reverseSum = await _calculatorGrain.Add(x, 0);
        //         return (x == forwardSum) && (x == reverseSum);
        //     }
        //
        //     return identityCheck().Result;
        // }
        //
        // /// <summary>
        // /// This is a property-based test.
        // ///
        // /// The test-runner will generate a large number of random values for
        // /// <paramref name="l"/> and <paramref name="r"/> each time the test is run,
        // /// and we will test a **property** of the addition operation.
        // ///
        // /// Mathematically, addition is the *only* operation to be
        // ///     * associative
        // ///     * commutative
        // ///     * have an identity of zero
        // /// so checking these three properties proves the addition operation is correct.
        // ///
        // /// In this test, we check that the property of commutativity is satisfied
        // ///
        // /// </summary>
        // /// <param name="l"></param>
        // /// <param name="r"></param>
        // /// <returns>`true` if the test should pass, `false` otherwise</returns>
        // [Property]
        // public bool AdditionIsCommutative(int l, int r)
        // {
        //     // Never call `.Result` on a grain call!
        //     // Use the trick of `await`ing in a local `async` function if possible!
        //     async Task<bool> commutativityCheck()
        //     {
        //         var forwardSum = await _calculatorGrain.Add(l, r);
        //         var reverseSum = await _calculatorGrain.Add(r, l);
        //         return forwardSum == reverseSum;
        //     }
        //
        //     return commutativityCheck().Result;
        // }
        //
        // /// <summary>
        // /// This is a property-based test.
        // ///
        // /// The test-runner will generate a large number of random values for
        // /// <paramref name="l"/> and <paramref name="r"/> each time the test is run,
        // /// and we will test a **property** of the addition operation.
        // ///
        // /// Mathematically, addition is the *only* operation to be
        // ///     * associative
        // ///     * commutative
        // ///     * have an identity of zero
        // /// so checking these three properties proves the addition operation is correct.
        // ///
        // /// In this test, we check that the property of associativity is satisfied
        // ///
        // /// </summary>
        // /// <param name="l"></param>
        // /// <param name="r"></param>
        // /// <returns>`true` if the test should pass, `false` otherwise</returns>
        // [Property]
        // public bool AdditionIsAssociative(int x1, int x2, int x3)
        // {
        //     // Never call `.Result` on a grain call!
        //     // Use the trick of `await`ing in a local `async` function if possible!
        //     async Task<bool> associativityCheck()
        //     {
        //         var forwardSum = await _calculatorGrain.Add(x1, await _calculatorGrain.Add(x2, x3));
        //         var reverseSum = await _calculatorGrain.Add(await _calculatorGrain.Add(x1, x2), x3);
        //         return forwardSum == reverseSum;
        //     }
        //
        //     return associativityCheck().Result;
        // }
    }
}
