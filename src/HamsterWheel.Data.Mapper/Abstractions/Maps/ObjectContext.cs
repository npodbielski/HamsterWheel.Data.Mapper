namespace HamsterWheel.Data.Mapper.Maps;

public record ObjectContext(Type Type, object? Instance)
{
    public static ObjectContext For(Type obj) => new(obj, null);
    public static ObjectContext For(object? obj) => new(obj?.GetType() ?? typeof(object), obj);
}