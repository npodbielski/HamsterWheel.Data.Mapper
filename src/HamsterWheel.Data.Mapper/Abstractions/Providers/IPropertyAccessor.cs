using HamsterWheel.Data.Mapper.Maps;

namespace HamsterWheel.Data.Mapper.Providers;

public interface IPropertyAccessor
{
    Type PropertyType { get; }
    PropertyPath Path { get; }
    Func<object?, object?>? Getter { get; }
    Action<object, object?>? Setter { get; }
    Type? DeclarationType { get; }
}