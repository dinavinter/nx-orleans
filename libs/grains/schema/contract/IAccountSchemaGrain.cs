using System.Text.Json;
using System.Threading.Tasks;
using Orleans;
using Orleans.Concurrency;

namespace grains.schema.contract;

public interface IAccountSchemaGrain:IGrainWithStringKey
{
  Task<SchemaValidationResult> ValidateAsync(Immutable<JsonElement> instance);
}
