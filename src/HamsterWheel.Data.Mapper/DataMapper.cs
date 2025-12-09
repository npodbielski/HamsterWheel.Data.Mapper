using System.Diagnostics.CodeAnalysis;
using HamsterWheel.Data.Mapper.Maps;
using HamsterWheel.Data.Mapper.Setup;

namespace HamsterWheel.Data.Mapper;

public static class DataMapper
{
    public const string AnyProperty = "*";
    public const string EntireSource = ".";
    public const char AnyPropertyChar = '*';

    public static Map AnyToAnyMap => [(AnyProperty, AnyProperty)];
    private static IDataMapper Instance { get; } = Services.Get<IDataMapper>();

    public static void Map(
        [DynamicallyAccessedMembers(IDataMapper.PropsAndCtors)]
        object? source,
        [DynamicallyAccessedMembers(IDataMapper.PropsAndCtors)]
        object destination,
        Map? map = null,
        IEnumerable<PropertyMapping>? cachedMap = null) =>
        Instance.Map(source, destination, map, cachedMap);

    public static IEnumerable<PropertyMapping> BuildMap<TSource, TDestination>(Map? map = null) =>
        Instance.BuildMap<TSource, TDestination>();

    public static IEnumerable<PropertyMapping> BuildMap(Type source, Type destination, Map? map = null) =>
        Instance.BuildMap(source, destination, map);

    public static IEnumerable<PropertyMapping> BuildMap(object? source, object destination, Map? map = null) =>
        Instance.BuildMap(source, destination, map);
}