using System.Threading.Tasks;
using grains.Contract;
using Orleans;

namespace grains.Implementation
{
    public class CalculatorGrain : Grain, ICalculatorGrain
    {
        public Task<int> Add(int l, int r) =>
            Task.FromResult(l + r);
    }
}
