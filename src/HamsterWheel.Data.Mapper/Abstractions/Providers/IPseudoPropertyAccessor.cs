using HamsterWheel.Data.Mapper.Maps;

namespace HamsterWheel.Data.Mapper.Providers;

public interface IPseudoPropertyAccessor
{
    IPropertyAccessor? CreateForType(Type type, PropertyPathChunk chunk);
}