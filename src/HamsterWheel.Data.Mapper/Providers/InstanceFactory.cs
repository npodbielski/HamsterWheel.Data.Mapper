using HamsterWheel.Utils;

namespace HamsterWheel.Data.Mapper.Providers;

public class InstanceFactory : IInstanceFactory
{
    public object? Create(Type type)
    {
        if (type.IsNullable())
        {
            return null;
        }

        if (type == typeof(string))
        {
            return string.Empty;
        }

        return Activator.CreateInstance(type) ?? throw new CreationOfTypeFailedException(type);
    }
}