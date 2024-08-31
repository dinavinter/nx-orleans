using System;
using System.Linq;
using System.Threading.Tasks;
using grains.Contract;
using Orleans;
using Orleans.Runtime;

namespace grains.Implementation;

public record StateWithHistory()
{
  public int Value => History.LastOrDefault();
  private int[] History { get; set; } = Array.Empty<int>();
  public void Update( Func<int, int> update)
  {
    History = History.Concat(new[] { update(Value) }).ToArray();
  }
  public void Undo()
  {
    History = History.Take(int.Max(History.Length - 1, 0)).ToArray();
  }
}


public class CalculatorGrain : Grain, ICalculatorGrain
{
  private readonly IPersistentState<StateWithHistory> _persistent;

  public CalculatorGrain([PersistentState("calc")] IPersistentState<StateWithHistory> persistent)
  {
    _persistent = persistent;
  }

  public Task<int> Add(int value)
  {
    _persistent.State.Update(v => v + value);
    return Task.FromResult(_persistent.State.Value);
  }

  public Task<int> Subtract(int value)
  {
    _persistent.State.Update(v => v - value);
    return Task.FromResult(_persistent.State.Value);
  }

  public Task<int> Undo()
  {
    _persistent.State.Undo();
    return Task.FromResult(_persistent.State.Value);
  }

  public Task<int> Get()
  {
    return Task.FromResult(_persistent.State.Value);
  }
}
