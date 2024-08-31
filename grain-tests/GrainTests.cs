using FsCheck.Xunit;
using Orleans.Contrib.UniversalSilo;
using Orleans.TestingHost;
using System;
using System.Threading.Tasks;
using grains.Contract;
using Xunit;

namespace grains.tests;

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
  private readonly TestCluster _cluster;
  private readonly ICalculatorGrain _calculatorGrain;

  public GrainTests(ClusterFixture fixture)
  {
    // extract the TestCluster instance here and save it...
    _cluster = fixture?.Cluster ?? throw new ArgumentNullException(nameof(fixture));

    // create a single grain to test if you want here, or alternately create a grain in the test itself
    _calculatorGrain = _cluster.GrainFactory.GetGrain<ICalculatorGrain>(Guid.NewGuid().ToString());
  }

  /// <summary>
  /// This is a traditional unit test.
  ///
  /// Provide known inputs and check the actual result against a known expected value
  /// </summary>
  /// <returns></returns>
  [Fact]
  public async Task KnownNumbersAreAddedCorrectly()
  {
    // this is an example of creating a grain within the test from using the TestCluster instance
    var adderGrain = _cluster.GrainFactory.GetGrain<ICalculatorGrain>(Guid.NewGuid().ToString());
    await adderGrain.Add(1);
    await adderGrain.Add(2);
    var result = await adderGrain.Get();
    Assert.Equal(3, result);
  }
}
