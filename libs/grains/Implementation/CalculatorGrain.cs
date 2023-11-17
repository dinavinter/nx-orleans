using Orleans;
using System.Threading.Tasks;
using SchemaLand.Api.Contract;

namespace SchemaLand.Api.Implementation
{
    public class CalculatorGrain : Grain, ICalculatorGrain
    {
        public Task<int> Add(int l, int r) =>
            Task.FromResult(l + r);
    }
}
