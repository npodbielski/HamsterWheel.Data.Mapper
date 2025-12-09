using System.Collections.ObjectModel;
using HamsterWheel.Data.Mapper.Providers;

namespace HamsterWheel.Data.Mapper.Maps;

using PropertiesMap = ReadOnlyCollection<PropertyMapping>;

public class MapBuilder(
    IPropertiesMatcher propertiesMatcher,
    IPropertyAccessorFactory propertyAccessorFactory) : IMapBuilder
{
    public PropertiesMap BuildMap(ObjectContext source, ObjectContext destination, Map map)
    {
        List<PropertyMapping> fullMap = [];
        foreach (var propMap in map)
        {
            var sourcePath = PropertyPath.From(propMap.Source);
            var destPath = PropertyPath.From(propMap.Destination);
            sourcePath = AddWildcardsIfNeeded(sourcePath, propMap, ref destPath);

            var sources = ExpandSourceWildcards(source, sourcePath);
            if (sources.Length <= 0)
            {
                continue;
            }

            var destinations = ExpandDestinationWildcards(destination, destPath);
            if (destinations.Length <= 0)
            {
                continue;
            }

            if (sourcePath.IsSource)
            {
                fullMap.Add(new PropertyMapping(sources[0], destinations[0]));
                return fullMap.AsReadOnly();
            }

            //if wildcard any '*', then match properties by names and take most similar
            if (sourcePath.Chunks.Last().IsAny || destPath.Chunks.Last().IsAny)
            {
                foreach (var sourceProp in sources)
                {
                    var destinationProp =
                        destinations.Select(d =>
                            {
                                var similarity = d.Path.Chunks.Reverse()
                                    .Zip(sourceProp.Path.Chunks.Reverse(), (dc, sc) => dc == sc ? 1 : 0).Sum(i => i);
                                return (similarity, destination: d);
                            }).Where(p => p.similarity > 0).OrderByDescending(p => p.similarity)
                            .Select(p => p.destination)
                            .FirstOrDefault();
                    if (destinationProp != null)
                    {
                        fullMap.Add(new PropertyMapping(sourceProp, destinationProp));
                    }
                }
            }
            //if no wildcard -> custom mapping should be 1:1 or unwinded properties are 1 to 1
            else
            {
                //if the length of both collections is the same; if not probably, a wildcard map is ambiguous
                if (sources.Length == destinations.Length)
                {
                    fullMap.AddRange(sources.Zip(destinations, (s, d) => new PropertyMapping(s, d)));
                }
            }
        }

        return fullMap.AsReadOnly();

        static PropertyPath AddWildcardsIfNeeded(PropertyPath sourcePath, (string Source, string Destination) propMap,
            ref PropertyPath destPath)
        {
            if (sourcePath.IsSource)
            {
                return sourcePath;
            }

            switch (sourcePath.Chunks.Last().IsAny)
            {
                case true when !destPath.Chunks.Last().IsAny:
                    destPath = PropertyPath.From($"{propMap.Destination}.*");
                    break;
                case false when destPath.Chunks.Last().IsAny && !sourcePath.Chunks.Last().HaveWildcard:
                    sourcePath = PropertyPath.From($"{propMap.Source}.*");
                    break;
            }

            return sourcePath;
        }
    }

    private IPropertyAccessor[] ExpandSourceWildcards(ObjectContext source, PropertyPath path)
    {
        if (path.IsSource)
        {
            return [new PropertyAccessor(source.Type, source.Type, path, o => o, null)];
        }

        var sources = new List<IPropertyAccessor>();
        var sourceType = source.Instance?.GetType() ?? source.Type;
        foreach (var chunk in path.Chunks)
        {
            IPropertyAccessor[] currentLevelProps;
            if (sources.Count == 0)
            {
                currentLevelProps = GetPropsForChunk(chunk, sourceType);
            }
            else
            {
                currentLevelProps = sources
                    .SelectMany(d =>
                        GetPropsForChunk(chunk, d.PropertyType).Select(i => propertyAccessorFactory.ToSourceProp(d, i)))
                    .ToArray();
                sources.Clear();
            }

            if (!chunk.IsAny)
            {
                sources.AddRange(currentLevelProps);
                continue;
            }

            foreach (var property in currentLevelProps)
            {
                var nestedProperties = UnwindSourcePocoClasses(chunk, property);
                if (nestedProperties != null)
                {
                    sources.AddRange(nestedProperties);
                }
                else
                {
                    sources.Add(property);
                }
            }
        }

        return sources.ToArray();

        IPropertyAccessor[] GetPropsForChunk(PropertyPathChunk chunk, Type type) =>
            chunk.HaveWildcard ? GetIntermediateProperties(type, chunk) : [propertyAccessorFactory.Create(type, chunk, mustHaveGetter: true)];
    }

    private IPropertyAccessor[] ExpandDestinationWildcards(ObjectContext destination, PropertyPath path)
    {
        var destinations = new List<IPropertyAccessor>();
        var destinationType =
            destination.Instance?.GetType() ?? destination.Type;
        foreach (var chunk in path.Chunks)
        {
            IPropertyAccessor[] currentLevelProps;
            if (destinations.Count == 0)
            {
                currentLevelProps = GetPropsForChunk(chunk, destinationType);
            }
            else
            {
                currentLevelProps = destinations
                    .SelectMany(s =>
                        GetPropsForChunk(chunk, s.PropertyType).Select(i => propertyAccessorFactory.ToDestProp(s, i)))
                    .ToArray();
                destinations.Clear();
            }

            if (!chunk.IsAny)
            {
                destinations.AddRange(currentLevelProps);
                continue;
            }

            foreach (var property in currentLevelProps)
            {
                var nestedProperties = UnwindDestinationPocoClasses(chunk, property);
                if (nestedProperties != null)
                {
                    destinations.AddRange(nestedProperties);
                }
                else
                {
                    destinations.Add(property);
                }
            }
        }

        return destinations.ToArray();

        IPropertyAccessor[] GetPropsForChunk(PropertyPathChunk chunk, Type type) =>
            chunk.HaveWildcard ? GetNestedDestinationProperties(type, chunk) : [propertyAccessorFactory.Create(type, chunk, mustHaveSetter: true)];
    }

    internal IPropertyAccessor[]? UnwindSourcePocoClasses(PropertyPathChunk chunk, IPropertyAccessor property) =>
        chunk is { IsAny: true, IsSource: false } && property.PropertyType.IsCustomType()
            ? GetIntermediateProperties(property.PropertyType, chunk)
                .Select(i => propertyAccessorFactory.ToSourceProp(property, i)).ToArray()
            : null;

    internal IPropertyAccessor[]? UnwindDestinationPocoClasses(PropertyPathChunk chunk,
        IPropertyAccessor property) =>
        chunk is { IsAny: true, IsSource: false } && property.PropertyType.IsCustomType()
            ? GetIntermediateProperties(property.PropertyType, chunk)
                .Select(i => propertyAccessorFactory.ToDestProp(property, i)).ToArray()
            : null;

    private IPropertyAccessor[] GetIntermediateProperties(Type sourceType, PropertyPathChunk chunk) =>
        propertiesMatcher.MatchProperties(sourceType, chunk).Where(p => p.Getter is not null).ToArray();

    private IPropertyAccessor[] GetNestedDestinationProperties(Type sourceType, PropertyPathChunk chunk) =>
        propertiesMatcher.MatchProperties(sourceType, chunk).Where(p => p.Setter is not null).ToArray();
}