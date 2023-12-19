using System.Text.Json;
using System.Threading.Tasks;
using grains.schema.contract;
using Orleans;
using Orleans.Concurrency;

namespace grains.schema;

[StatelessWorker, Reentrant]
public class AccountSchemaGrain : Grain, IAccountSchemaGrain
{
  private AccountsSchema _schema;

  /// <inheritdoc />
  public override Task OnActivateAsync()
  {
    _schema = new AccountsSchema();
    return base.OnActivateAsync();
  }

  public Task<SchemaValidationResult> ValidateAsync(Immutable<JsonElement> instance)
  {
    return _schema.ValidateAsync(instance.Value);
  }
}
