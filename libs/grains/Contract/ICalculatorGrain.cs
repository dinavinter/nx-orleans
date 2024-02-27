using System.Threading.Tasks;
using Orleans;

namespace grains.Contract
{
    public interface ICalculatorGrain : IGrainWithGuidKey
    {
        /// <summary>
        /// Adds two integers
        /// </summary>
        /// <param name="l">Integer to Add</param>
        /// <param name="r">Integer to Add</param>
        /// <returns>Sum of <see param="l"/> and <see param="r"/></returns>
        Task<int> Add(int l, int r);
    }
}
