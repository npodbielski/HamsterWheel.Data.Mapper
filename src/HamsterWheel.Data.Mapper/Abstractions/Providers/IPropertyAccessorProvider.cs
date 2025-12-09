namespace HamsterWheel.Data.Mapper.Providers;

public interface IPropertyAccessorProvider
{
    IPropertyAccessor? GetAccessor(Type type,string propertyName);
}