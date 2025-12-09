using System.Text.Json;
using HamsterWheel.Data.Mapper.Maps;

namespace HamsterWheel.Data.Mapper.Providers;

public class JsonDocumentPropertyAccessorProvider : IPropertyAccessorProvider
{
    private Type _supportedType = typeof(JsonDocument);

    public IPropertyAccessor? GetAccessor(Type type, string propertyName)
    {
        if (type.IsAssignableTo(typeof(JsonDocument)))
        {
            return new PropertyAccessor(type, typeof(object), PropertyPath.From(propertyName),
                o => ((JsonDocument)o!).RootElement.GetProperty(propertyName), null);
        }

        return null;
    }
}