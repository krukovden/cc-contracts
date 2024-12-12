using NJsonSchema;

namespace Abs.CommonCore.Drex.Contracts.ModelGenerator;

public class CustomTypeNameGenerator : ITypeNameGenerator
{
    public string Generate(JsonSchema schema, string typeNameHint, IEnumerable<string> reservedTypeNames)
    {
        return !string.IsNullOrWhiteSpace(schema.Title) ? schema.Title : typeNameHint;
    }
}
