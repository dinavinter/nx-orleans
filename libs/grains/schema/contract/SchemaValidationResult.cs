using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Json.Schema;
using Orleans.Concurrency;

namespace grains.schema.contract;

[Immutable]
public record SchemaValidationResult(EvaluationResults EvaluationResults)
{
  public bool IsValid => EvaluationResults.IsValid;

  [JsonPropertyName("validationErrors")]
  public IDictionary<string, string[]> Errors => EvaluationResults
    .Errors?.ToDictionary(x => x.Key, x => new[] { x.Value });

}
