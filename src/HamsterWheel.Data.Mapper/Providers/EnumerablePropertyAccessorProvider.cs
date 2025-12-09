using System.Collections;
using HamsterWheel.Data.Mapper.Maps;

namespace HamsterWheel.Data.Mapper.Providers;

public class EnumerablePropertyAccessorProvider : IPropertyAccessorProvider
{
    private Type _supportedType = typeof(IEnumerable);

    public IPropertyAccessor? GetAccessor(Type type, string propertyName)
    {
        if (type.IsAssignableTo(_supportedType) && int.TryParse(propertyName, out var index))
        {
            return new PropertyAccessor(_supportedType, typeof(object), PropertyPath.From(propertyName),
                o => ((IEnumerable)o!).Cast<object?>().ElementAt(index), null);
        }

        return null;
    }
}