using System.Text.Json;
using System.Text.Json.Nodes;
using HamsterWheel.Data.Mapper.Maps;

namespace HamsterWheel.Data.Mapper.Providers;

public class JsonNodePropertyAccessorProvider : IPropertyAccessorProvider
{
    private readonly Type _supportedType = typeof(JsonNode);

    public IPropertyAccessor? GetAccessor(Type type, string propertyName)
    {
        if (!type.IsAssignableTo(_supportedType))
        {
            return null;
        }

        return new PropertyAccessor(_supportedType, typeof(object), PropertyPath.From(propertyName),
            o => ((JsonNode)o!)[propertyName],
            (o, v) => ((JsonNode)o)[propertyName] = JsonNode.Parse(JsonSerializer.Serialize(v)));
    }
}