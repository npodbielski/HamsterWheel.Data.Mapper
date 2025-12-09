using System.IO.Enumeration;
using HamsterWheel.Data.Mapper.Providers;
using HamsterWheel.HLinq.Reflection;
using static HamsterWheel.Data.Mapper.DataMapper;

namespace HamsterWheel.Data.Mapper.Maps;

internal class PropertiesMatcher(IPropertiesCache propertiesCache, IPropertyAccessorFactory propertyAccessorFactory) : IPropertiesMatcher
{
    public IEnumerable<IPropertyAccessor> MatchProperties(Type sourceType, PropertyPathChunk chunk)
    {
        var allProperties = propertiesCache.From(sourceType);
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
                filter = s => s.StartsWith(chunk.Name.TrimEnd(AnyPropertyChar));
            }
            //any other filter with any number of * i.e. "F*Name*" which should match "FileName" but also "FirstNameOfUser" or "FullPathNameOfAvatar"
            else
            {
                filter = s => FileSystemName.MatchesSimpleExpression(chunk.Name, s);
            }
        }

        return allProperties
            .Where(p => filter is null || filter(p.Name))
            .Select(p => propertyAccessorFactory.Create(sourceType, p));
    }
}