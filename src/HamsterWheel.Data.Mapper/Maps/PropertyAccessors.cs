using HamsterWheel.Data.Mapper.Providers;

namespace HamsterWheel.Data.Mapper.Maps;

public record PropertyAccessor(
    Type? DeclarationType,
    Type PropertyType,
    PropertyPath Path,
    Func<object?, object?>? Getter,
    Action<object, object?>? Setter)
    : IPropertyAccessor
{
    public override string ToString() => string.Join(".", Path.Chunks.Select(c => c.Name));
}