using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.RegularExpressions;
using HamsterWheel.HLinq.Data.Converters;
using HamsterWheel.HLinq.Reflection;

namespace HamsterWheel.Data.Mapper;

using PropertiesMap = ReadOnlyCollection<PropertyMapping>;

public static class DataMapper
{
    public const string AnyProperty = "*";
    public const string EntireSource = ".";
    public const char AnyPropertyChar = '*';

    private const DynamicallyAccessedMemberTypes PropsAndCtors =
        DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.PublicParameterlessConstructor;

    private static readonly ConcurrentBag<PropertyAccessors> AccessorsCache = [];

    private static readonly PropertiesCache PropertiesCache = new();

    public static Map AnyToAnyMap => [(AnyProperty, AnyProperty)];

    public static void Map(
        [DynamicallyAccessedMembers(PropsAndCtors)]
        object? source,
        [DynamicallyAccessedMembers(PropsAndCtors)]
        object destination,
        Map? map = null,
        PropertiesMap? propertiesMap = null)
    {
        var fullMap = propertiesMap ?? BuildMap(source, destination, map);
        foreach (var mapping in fullMap)
        {
            if (mapping.Source.Getter is null)
            {
                throw new MissingPropertyGetterException();
            }

            if (mapping.Destination.Setter is null)
            {
                throw new MissingPropertySetterException();
            }

            var value = mapping.Source.Getter(source);
            mapping.Destination.Setter(destination, value);
        }
    }

    public static PropertiesMap BuildMap(object? source, object destination, Map? map = null) =>
        BuildMap(ObjectContext.For(source), ObjectContext.For(destination), map ?? AnyToAnyMap);

    private static PropertiesMap BuildMap(ObjectContext source, ObjectContext destination, Map map)
    {
        List<PropertyMapping> fullMap = [];
        foreach (var propMap in map)
        {
            var sourcePath = PropertyPath.From(propMap.Source);
            var destPath = PropertyPath.From(propMap.Destination);
            if (!sourcePath.IsSource)
            {
                switch (sourcePath.Chunks.Last().IsAny)
                {
                    case true when !destPath.Chunks.Last().IsAny:
                        destPath = PropertyPath.From($"{propMap.Destination}.*");
                        break;
                    case false when destPath.Chunks.Last().IsAny:
                        sourcePath = PropertyPath.From($"{propMap.Source}.*");
                        break;
                }
            }

            var sources = UnwindSourcePaths(source, sourcePath);
            if (sources.Length <= 0)
            {
                continue;
            }

            var destinations = UnwindDestinationPaths(destination, destPath);
            if (sourcePath.IsSource)
            {
                fullMap.Add(new PropertyMapping(sources[0], destinations[0]));
            }
            //if wildcard then match properties by names
            else if (sourcePath.Chunks.Last().HaveWildcard || destPath.Chunks.Last().HaveWildcard)
            {
                foreach (var sourceProp in sources)
                {
                    var destinationProp =
                        destinations.FirstOrDefault(d => d.Path.Chunks.Last() == sourceProp.Path.Chunks.Last());
                    if (destinationProp != null)
                    {
                        fullMap.Add(new PropertyMapping(sourceProp, destinationProp));
                    }
                }
            }
            //if no wildcard -> custom mapping should be 1:1
            else
            {
                fullMap.AddRange(sources.Zip(destinations, (s, d) => new PropertyMapping(s, d)));
            }
        }

        return fullMap.AsReadOnly();
    }

    private static PropertyAccessors[] UnwindSourcePaths(ObjectContext source, PropertyPath path)
    {
        var sources = new List<PropertyAccessors>();
        if (path.IsSource)
        {
            return [new PropertyAccessors(source.Type, source.Type, path, o => o, null)];
        }

        Type[] sourceTypes = [source.Instance?.GetType() ?? source.Type ?? throw new SourceTypeMissingException()];

        foreach (var chunk in path.Chunks)
        {
            if (sources.Count > 0)
            {
                sourceTypes = sources
                    .Where(s => s.PropertyType != null)
                    .Select(s => s.PropertyType).ToArray()!;
            }

            foreach (var sourceType in sourceTypes)
            {
                var subProperties = chunk.HaveWildcard
                    ? GetWildcardMatches(sourceType, chunk)
                    : [TryGetSourceFromCache(sourceType, chunk) ?? CreateSourceAccessor(sourceType, chunk)];

                if (sources.Count != 0)
                {
                    sources = sources.SelectMany(s => subProperties.Select(s.ToSourceProp)).ToList();
                }
                else
                {
                    sources.AddRange(subProperties);
                }
            }
        }

        return sources.ToArray();
    }

    private static PropertyAccessors[] UnwindDestinationPaths(ObjectContext destination, PropertyPath path)
    {
        var destinations = new List<PropertyAccessors>();
        Type[] destinationTypes = [destination.Instance?.GetType() ?? destination.Type?? throw new DestinationTypeMissingException()];
        foreach (var chunk in path.Chunks)
        {
            if (destinations.Count > 0)
            {
                destinationTypes = destinations
                    .Where(s => s.PropertyType != null)
                    .Select(s => s.PropertyType).ToArray()!;
            }

            foreach (var destType in destinationTypes)
            {
                var subProperties = chunk.HaveWildcard
                    ? MatchesDestinationProperties(destType, chunk)
                    : [TryGetDestinationFromCache(destType, chunk) ?? CreateDestinationAccessor(destType, chunk)];

                if (destinations.Count != 0)
                {
                    destinations = destinations.SelectMany(s => subProperties.Select(s.ToDestProp)).ToList();
                }
                else
                {
                    destinations.AddRange(subProperties);
                }
            }
        }

        return destinations.ToArray();
    }

    private static PropertyAccessors[] GetWildcardMatches(Type sourceType, PropertyPathChunk chunk) =>
        MatchProperties(sourceType, chunk).Where(p => p.Getter is not null).ToArray();

    private static PropertyAccessors[] MatchesDestinationProperties(Type sourceType, PropertyPathChunk chunk) =>
        MatchProperties(sourceType, chunk).Where(p => p.Setter is not null).ToArray();

    private static IEnumerable<PropertyAccessors> MatchProperties(Type sourceType, PropertyPathChunk chunk)
    {
        var allProperties = PropertiesCache.From(sourceType);
        Func<string, bool>? filter = null;
        //if any prop filter is null and all properties are returned
        if (!chunk.IsAny)
        {
            //i.e. "*Name" which should match "FullName", "SurName", etc.
            if (chunk.Name.StartsWith(AnyPropertyChar) && chunk.Name.Count(c => c == AnyPropertyChar) == 1)
            {
                filter = s => s.EndsWith(chunk.Name.TrimStart(AnyPropertyChar));
            }
            //i.e. "Dir*" which should match "DirPath", "DirExtensions", etc.
            else if (chunk.Name.EndsWith(AnyPropertyChar) && chunk.Name.Count(c => c == AnyPropertyChar) == 1)
            {
                filter = s => s.EndsWith(chunk.Name.TrimEnd(AnyPropertyChar));
            }
            //any other filter with any number of * i.e. "F*Name*" which should match "FileName" but also "FirstNameOfUser" or "FullPathNameOfAvatar"
            else
            {
                var regex = new Regex(chunk.Name.Replace(AnyPropertyChar.ToString(), ".*?"));
                filter = s => regex.IsMatch(s);
            }
        }

        var where = allProperties
            .Where(p => filter is null || filter(p.Name))
            .Select(p => CreatePropertyAccessor(sourceType, p));
        return where;
    }

    private static PropertyAccessors CreatePropertyAccessor(Type sourceType, PropertyInfo property)
    {
        var getter = property.GetMethod is not null
            ? (Func<object, object?>)(o => property.GetMethod!.Invoke(o, null)!)
            : null;
        var setter = property.SetMethod is not null
            ? (Action<object, object?>)((o, v) =>
            {
                if (DefaultConverter.Instance.NeedConversion(property.PropertyType, v))
                {
                    v = DefaultConverter.Instance.ConvertTo(property.PropertyType, v);
                }

                property.SetMethod?.Invoke(o, [v]);
            })
            : null;
        var source = new PropertyAccessors(sourceType, property.PropertyType, PropertyPath.From(property.Name), getter,
            setter);
        AccessorsCache.Add(source);
        return source;
    }

    private static PropertyAccessors CreateSourceAccessor(Type sourceType, PropertyPathChunk chunk)
    {
        var source = CreatePropertyAccessor(sourceType, chunk);
        return source.Getter is null ? throw new MissingPropertyGetterException() : source;
    }

    private static PropertyAccessors CreateDestinationAccessor(Type destType, PropertyPathChunk chunk)
    {
        var source = CreatePropertyAccessor(destType, chunk);
        return source.Setter is null ? throw new MissingPropertySetterException() : source;
    }

    private static PropertyAccessors CreatePropertyAccessor(Type type, PropertyPathChunk chunk)
    {
        var property = PropertiesCache.Single(type, chunk.Name);
        if (property == null)
        {
            //TODO: try dictionaries, JsonNode and etc
            throw new MissingPropertyException(type, chunk.Name);
        }

        return CreatePropertyAccessor(type, property);
    }

    private static PropertyAccessors? TryGetSourceFromCache(Type sourceType, PropertyPathChunk path)
    {
        if (path.HaveWildcard)
        {
            throw new FetchingFromCacheWithWildcardException();
        }

        return AccessorsCache.FirstOrDefault(c =>
            c.DeclarationType == sourceType && c.Path.Chunks.Length == 1 && c.Path.Chunks[0].Name == path.Name);
    }

    private static PropertyAccessors? TryGetDestinationFromCache(Type sourceType, PropertyPathChunk path)
    {
        if (path.HaveWildcard)
        {
            throw new FetchingFromCacheWithWildcardException();
        }

        return AccessorsCache.FirstOrDefault(c =>
            c.DeclarationType == sourceType && c.Path.Chunks.Length == 1 && c.Path.Chunks[0].Name == path.Name);
    }

    //TODO: add delegates creation from old library DelegatesFactory
    //...or we could add code generation for properties and constructors
    internal static object TryProducingNewInstanceOfType(Type type)
    {
        //TODO: check if this is possible to handle constructor parameters via arguments of the activator
        return Activator.CreateInstance(type) ?? throw new CreationOfTypeFailedException(type);
    }
}