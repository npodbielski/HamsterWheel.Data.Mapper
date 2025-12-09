using HamsterWheel.Data.Mapper.Providers;

namespace HamsterWheel.Data.Mapper.Maps;

internal class PseudoPropertyAccessor(IEnumerable<IPropertyAccessorProvider> providers) : IPseudoPropertyAccessor
{
    private IPropertyAccessorProvider[] Providers { get; } = providers.ToArray();

    public IPropertyAccessor? CreateForType(Type type, PropertyPathChunk chunk) =>
        Providers.Select(p => p.GetAccessor(type, chunk.Name)).FirstOrDefault(a => a != null);
}