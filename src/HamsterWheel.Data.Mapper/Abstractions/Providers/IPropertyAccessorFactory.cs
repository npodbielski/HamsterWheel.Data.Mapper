using System.Reflection;
using HamsterWheel.Data.Mapper.Maps;

namespace HamsterWheel.Data.Mapper.Providers;

public interface IPropertyAccessorFactory
{
    IPropertyAccessor Create(Type sourceType, PropertyInfo property);
    IPropertyAccessor ToSourceProp(IPropertyAccessor parentProp, IPropertyAccessor subProp);
    IPropertyAccessor ToDestProp(IPropertyAccessor parentProp, IPropertyAccessor subProp);
    IPropertyAccessor Create(Type sourceType, PropertyPathChunk chunk, bool mustHaveSetter = false, bool mustHaveGetter = false);
}