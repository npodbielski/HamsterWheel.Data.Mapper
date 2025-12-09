using System.Reflection;
using HamsterWheel.Data.Mapper.Providers;
using HamsterWheel.HLinq.Data.Converters;
using HamsterWheel.HLinq.Reflection;

namespace HamsterWheel.Data.Mapper.Maps;

public class PropertyAccessorFactory(
    IPropertiesCache propertiesCache,
    IInstanceFactory instanceFactory,
    IAccessorCache accessorCache,
    IPseudoPropertyAccessor pseudoPropertyAccessor)
    : IPropertyAccessorFactory
{
    public IPropertyAccessor Create(Type sourceType, PropertyPathChunk chunk, bool mustHaveSetter = false,
        bool mustHaveGetter = false)
    {
        var fromCache = TryGetFromCache(sourceType, chunk);
        if (fromCache != null)
        {
            return fromCache;
        }

        var source = CreatePropertyAccessor(sourceType, chunk);
        if (mustHaveGetter && source.Getter is null)
        {
            throw new MissingPropertyGetterException(source.ToString());
        }

        if (mustHaveSetter && source.Setter is null)
        {
            throw new MissingPropertySetterException(source.ToString());
        }

        return source;
    }

    public IPropertyAccessor CreatePropertyAccessor(Type type, PropertyPathChunk chunk)
    {
        var property = propertiesCache.Single(type, chunk.Name);
        if (property != null)
        {
            return Create(type, property);
        }

        return pseudoPropertyAccessor.CreateForType(type, chunk) ??
               throw new MissingPropertyException(type, chunk.Name);
    }

    public IPropertyAccessor? TryGetFromCache(Type sourceType, PropertyPathChunk path) => path.HaveWildcard
        ? throw new FetchingFromCacheWithWildcardException()
        : accessorCache.Get(sourceType, path.Name);

    public IPropertyAccessor Create(Type sourceType, PropertyInfo property)
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
        var source = new PropertyAccessor(sourceType, property.PropertyType, PropertyPath.From(property.Name), getter!,
            setter!);
        accessorCache.Add(source);
        return source;
    }

    public IPropertyAccessor ToSourceProp(IPropertyAccessor parentProp, IPropertyAccessor subProp)
    {
        var newChunks = new PropertyPath(parentProp.Path.Chunks.Concat(subProp.Path.Chunks).ToArray());
        if (parentProp.Getter is null)
        {
            throw new RootSourcePropertyGetterNullException();
        }

        if (subProp.Getter is null)
        {
            throw new IntermediateSourcePropertyGetterNullException();
        }

        return new PropertyAccessor(parentProp.DeclarationType, subProp.PropertyType, newChunks, o =>
        {
            var intermediateValue = parentProp.Getter(o);
            intermediateValue = GuardIntermediateValue(parentProp, subProp, intermediateValue, o!);
            return subProp.Getter(intermediateValue);
        }, (o, v) => subProp.Setter!(parentProp.Getter(o)!, v));
    }

    public IPropertyAccessor ToDestProp(IPropertyAccessor parentProp, IPropertyAccessor subProp)
    {
        var newChunks = new PropertyPath(parentProp.Path.Chunks.Concat(subProp.Path.Chunks).ToArray());
        if (parentProp.Getter is null)
        {
            throw new RootDestinationPropertyGetterNullException();
        }

        if (subProp.Setter is null)
        {
            throw new IntermediateDestinationPropertySetterNullException();
        }

        return new PropertyAccessor(parentProp.DeclarationType, subProp.PropertyType, newChunks,
            o => subProp.Getter!(parentProp.Getter(o)), (o, v) =>
            {
                var intermediateValue = parentProp.Getter(o);
                intermediateValue = GuardIntermediateValue(parentProp, subProp, intermediateValue, o);
                subProp.Setter(intermediateValue, v);
            }
        );
    }

    private object GuardIntermediateValue(IPropertyAccessor parentProp, IPropertyAccessor subPropAccessor,
        object? intermediateValue, object o)
    {
        if (intermediateValue != null)
        {
            return intermediateValue;
        }

        intermediateValue = instanceFactory.Create(parentProp.PropertyType);
        if (parentProp.Setter is not null)
        {
            parentProp.Setter(o, intermediateValue);
        }
        else
        {
            throw new CantSetNewInstanceOfTypeAtPropertyPathException(parentProp.Path.Chunks.Last().Name,
                subPropAccessor.PropertyType, parentProp.DeclarationType!);
        }

        return intermediateValue!;
    }

    internal class CantSetNewInstanceOfTypeAtPropertyPathException(string? propertyPath, Type type, Type declaringType)
        : DataMapperException(
            $"Can not set new instance of type '{type.FullName}' as property '{propertyPath}' of '{declaringType.FullName}' type. Most probable cause is private setter or compute only property.");

    internal class IntermediateDestinationPropertySetterNullException()
        : DataMapperException("Cannot create nested destination property if nested property setter is null");

    internal class IntermediateSourcePropertyGetterNullException()
        : DataMapperException("Cannot create nested source property if intermediate getter is null");

    internal class RootSourcePropertyGetterNullException()
        : DataMapperException("Cannot create nested source property if root property getter is null");

    internal class RootDestinationPropertyGetterNullException()
        : DataMapperException("Cannot create nested source property if root property getter is null");
}