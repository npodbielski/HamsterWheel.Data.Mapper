using System.Collections.Concurrent;

namespace HamsterWheel.Data.Mapper.Providers;

public class AccessorCache : IAccessorCache
{
    private readonly ConcurrentBag<IPropertyAccessor> _bag = [];

    public IPropertyAccessor? Get(Type type, string propertyName) =>
        _bag.FirstOrDefault(c =>
            c.DeclarationType == type && c.Path.Chunks.Length == 1 && c.Path.Chunks[0].Name == propertyName);
    
    public void Add(IPropertyAccessor accessor) => _bag.Add(accessor);
}