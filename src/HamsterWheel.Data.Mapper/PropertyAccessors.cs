namespace HamsterWheel.Data.Mapper;

public record PropertyAccessors(
    Type? DeclarationType,
    Type? PropertyType,
    PropertyPath Path,
    Func<object, object?>? Getter,
    Action<object, object?>? Setter)
{
    public PropertyAccessors ToSourceProp(PropertyAccessors subProp)
    {
        var newChunks = new PropertyPath(Path.Chunks.Concat(subProp.Path.Chunks).ToArray());
        if (Getter is null)
        {
            throw new RootSourcePropertyGetterNullException();
        }

        if (subProp.Getter is null)
        {
            throw new IntermediateSourcePropertyGetterNullException();
        }

        return this with
        {
            Path = newChunks, Getter = o =>
            {
                var intermediateValue = Getter(o);
                intermediateValue = GuardIntermediateValue(intermediateValue, o);
                return subProp.Getter(intermediateValue);
            },
            PropertyType = subProp.PropertyType
        };
    }

    public PropertyAccessors ToDestProp(PropertyAccessors subProp)
    {
        var newChunks = new PropertyPath(Path.Chunks.Concat(subProp.Path.Chunks).ToArray());
        if (Getter is null)
        {
            throw new RootDestinationPropertyGetterNullException();
        }

        if (subProp.Setter is null)
        {
            throw new IntermediateDestinationPropertySetterNullException();
        }

        return this with
        {
            Path = newChunks, Setter = (o, v) =>
            {
                var intermediateValue = Getter(o);
                intermediateValue = GuardIntermediateValue(intermediateValue, o);
                subProp.Setter(intermediateValue, v);
            },
            PropertyType = subProp.PropertyType
        };
    }

    private object GuardIntermediateValue(object? intermediateValue, object o)
    {
        if (intermediateValue == null)
        {
            intermediateValue =
                DataMapper.TryProducingNewInstanceOfType(PropertyType ?? throw new SourceTypeMissingException());
            if (Setter is not null)
            {
                Setter(o, intermediateValue);
            }
            else
            {
                throw new CantSetNewInstanceOfTypeAtPropertyPathException(Path.Chunks.Last().Name, PropertyType,
                    DeclarationType);
            }
        }

        return intermediateValue;
    }


    private class IntermediateSourcePropertyGetterNullException()
        : DataMapperException("Cannot create nested source property if intermediate getter is null");

    private class RootSourcePropertyGetterNullException()
        : DataMapperException("Cannot create nested source property if root property getter is null");

    private class IntermediateDestinationPropertySetterNullException()
        : DataMapperException("Cannot create nested destination property if nested property setter is null");

    private class RootDestinationPropertyGetterNullException()
        : DataMapperException("Cannot create nested source property if root property getter is null");
}