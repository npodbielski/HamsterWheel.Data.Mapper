using System.Diagnostics.CodeAnalysis;
using HamsterWheel.Data.Mapper.Maps;

namespace HamsterWheel.Data.Mapper;

public interface IDataMapper
{
    public const DynamicallyAccessedMemberTypes PropsAndCtors =
        DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.PublicParameterlessConstructor;

    void Map(
        [DynamicallyAccessedMembers(PropsAndCtors)]
        object? source,
        [DynamicallyAccessedMembers(PropsAndCtors)]
        object destination,
        Map? map = null,
        IEnumerable<PropertyMapping>? cachedMap = null);

    IEnumerable<PropertyMapping> BuildMap<TSource, TDestination>(Map? map = null);
    IEnumerable<PropertyMapping> BuildMap(Type source, Type destination, Map? map = null);
    IEnumerable<PropertyMapping> BuildMap(object? source, object destination, Map? map = null);
}