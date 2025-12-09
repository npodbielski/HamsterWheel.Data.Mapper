namespace HamsterWheel.Data.Mapper.Providers;

public interface IAccessorCache
{
    IPropertyAccessor? Get(Type type, string propertyName);
    void Add(IPropertyAccessor accessor);
}