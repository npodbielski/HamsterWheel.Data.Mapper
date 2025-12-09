using System.Diagnostics.CodeAnalysis;
using HamsterWheel.Data.Mapper.Maps;

namespace HamsterWheel.Data.Mapper;

using static DataMapper;

public class DataMapperImplementation(IMapBuilder mapBuilder) : IDataMapper
{
    public void Map(
        [DynamicallyAccessedMembers(IDataMapper.PropsAndCtors)]
        object? source,
        [DynamicallyAccessedMembers(IDataMapper.PropsAndCtors)]
        object destination,
        Map? map = null,
        IEnumerable<PropertyMapping>? cachedMap = null)
    {
        var fullMap = cachedMap ?? BuildMap(source, destination, map);
        foreach (var mapping in fullMap)
        {
            if (mapping.Source.Getter is null)
            {
                throw new MissingPropertyGetterException(mapping.Source.ToString());
            }

            if (mapping.Destination.Setter is null)
            {
                throw new MissingPropertySetterException(mapping.Destination.ToString());
            }

            var value = mapping.Source.Getter(source);
            mapping.Destination.Setter(destination, value);
        }
    }

    public IEnumerable<PropertyMapping> BuildMap<TSource, TDestination>(Map? map = null) =>
        BuildMap(typeof(TSource), typeof(TDestination), map ?? AnyToAnyMap);

    public IEnumerable<PropertyMapping> BuildMap(Type source, Type destination, Map? map = null) =>
        mapBuilder.BuildMap(ObjectContext.For(source), ObjectContext.For(destination), map ?? AnyToAnyMap);

    public IEnumerable<PropertyMapping> BuildMap(object? source, object destination, Map? map = null) =>
        mapBuilder.BuildMap(ObjectContext.For(source), ObjectContext.For(destination), map ?? AnyToAnyMap);
}