using System.Threading.Tasks;
using Orleans;
using Orleans.Concurrency;

namespace grains.Contract;

public interface ICalculatorGrain : IGrainWithStringKey
{
  /// <summary>
  /// Adds the given value to the current value
  /// </summary>
  /// <param name="value">an number value to add</param>
  /// <returns>the current state of the calculator</returns>
  Task<int> Add(int value);

  /// <summary>
  /// Substructs the given value from the current value
  /// </summary>
  /// <param name="value"></param>
  /// <returns>the current state of the calculator</returns>
  Task<int> Subtract(int value);


  /// <summary>
  /// Undo the last operation
  /// </summary>
  /// <returns>the current state of the calculator</returns>
  Task<int> Undo();

  /// <summary>
  /// Gets the current value
  /// </summary>
  /// <returns>the current state of the calculator</returns>
  Task<int> Get();
}
