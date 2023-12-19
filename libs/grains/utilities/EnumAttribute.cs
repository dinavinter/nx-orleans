using System;
using Json.Schema.Generation;
using Json.Schema.Generation.Intents;

namespace grains.utilities;

[AttributeUsage(AttributeTargets.Property)]
public class EnumAttribute : Attribute, IAttributeHandler<EnumAttribute>
{
  public string[] Values { get; }

  public EnumAttribute(params string[] Values)
  {
    this.Values = Values;
  }


  /// <inheritdoc />
  public void AddConstraints(SchemaGenerationContextBase context, Attribute attribute)
  {
    context.Intents.Add(new EnumIntent(Values));
  }
}
