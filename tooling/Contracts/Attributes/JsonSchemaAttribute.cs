namespace Contracts.Helpers.Attributes;

// ReSharper disable once UnusedType.Global
#pragma warning disable CA1018
public class JsonSchemaAttribute : Attribute
#pragma warning restore CA1018
{
    public string Schema { get; }

    public JsonSchemaAttribute(string schema)
    {
        Schema = schema;
    }
}
