using HamsterWheel.Data.Mapper.Maps;

namespace HamsterWheel.Data.Mapper.Providers;

public class DictionaryPropertyAccessorProvider : IPropertyAccessorProvider
{
    private readonly Type _supportedType = typeof(IDictionary<string, object>);

    public IPropertyAccessor? GetAccessor(Type type, string propertyName)
    {
        if (!type.IsAssignableTo(_supportedType))
        {
            return null;
        }

        return new PropertyAccessor(_supportedType, typeof(object), PropertyPath.From(propertyName),
            o => ((IDictionary<string, object?>)o!)[propertyName],
            (o, v) => ((IDictionary<string, object?>)o)[propertyName] = v);
    }
}