namespace Contracts.Helpers.Extensions;

// ReSharper disable once UnusedType.Global
// ReSharper disable once UnusedMember.Global
public static class JsonSchemaAttributeExtensions
{
    public static string ToJsonSchema(this object obj)
    {
        var rawAttribute = Attribute.GetCustomAttribute(obj.GetType(), typeof(JsonSchemaAttribute));
        return rawAttribute is not JsonSchemaAttribute jsonSchemaAttribute
            ? throw new Exception($"Type '{obj.GetType().FullName}' does not have {nameof(JsonSchemaAttribute)}")
            : jsonSchemaAttribute.Schema;
    }
}
