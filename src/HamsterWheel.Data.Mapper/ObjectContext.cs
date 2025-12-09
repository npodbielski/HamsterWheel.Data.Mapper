namespace HamsterWheel.Data.Mapper;

public record ObjectContext(Type? Type, object? Instance)
{
    public static ObjectContext For(object? obj) => new(obj?.GetType(), obj);
}