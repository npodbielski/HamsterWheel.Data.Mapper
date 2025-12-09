using HamsterWheel.Data.Mapper.Maps;
using HamsterWheel.Data.Mapper.Providers;

namespace HamsterWheel.Data.Mapper;

public interface IPropertiesMatcher
{
    IEnumerable<IPropertyAccessor> MatchProperties(Type sourceType, PropertyPathChunk chunk);
}