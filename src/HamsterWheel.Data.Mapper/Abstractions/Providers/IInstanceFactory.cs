namespace HamsterWheel.Data.Mapper.Providers;

public interface IInstanceFactory
{
    object? Create(Type type);
}