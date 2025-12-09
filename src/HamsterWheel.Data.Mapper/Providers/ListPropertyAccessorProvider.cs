using System.Collections;
using HamsterWheel.Data.Mapper.Maps;
using HamsterWheel.HLinq.Data.Converters;

namespace HamsterWheel.Data.Mapper.Providers;

public class ListPropertyAccessorProvider(IInstanceFactory instanceFactory) : IPropertyAccessorProvider
{
    private readonly Type _supportedType = typeof(IList);

    public IPropertyAccessor? GetAccessor(Type type, string propertyName)
    {
        if (!type.IsAssignableTo(_supportedType) || !int.TryParse(propertyName, out var index))
        {
            return null;
        }

        var itemType = type.IsGenericType ? type.GetGenericArguments()[0] : typeof(object);
        return new PropertyAccessor(_supportedType, typeof(object), PropertyPath.From(propertyName),
            o => ((IList)o!)[index],
            (o, v) =>
            {
                var list = (IList)o;
                while (list.Count <= index)
                {
                    list.Add(instanceFactory.Create(itemType));
                }

                list[index] = DefaultConverter.Instance.ConvertTo(itemType, v);
            });
    }
}