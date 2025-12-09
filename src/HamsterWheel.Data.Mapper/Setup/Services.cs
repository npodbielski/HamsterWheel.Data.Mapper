using HamsterWheel.Data.Mapper.Maps;
using HamsterWheel.Data.Mapper.Providers;
using HamsterWheel.HLinq.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace HamsterWheel.Data.Mapper.Setup;

public static class Services
{
    private static readonly IServiceProvider ServiceProvider =
        new ServiceCollection().ConfigureDataMapper().BuildServiceProvider();

    public static IServiceCollection ConfigureDataMapper(this IServiceCollection services) => services
        .AddSingleton<IPropertyAccessorProvider, DictionaryPropertyAccessorProvider>()
        .AddSingleton<IPropertyAccessorProvider, ListPropertyAccessorProvider>()
        .AddSingleton<IPropertyAccessorProvider, EnumerablePropertyAccessorProvider>()
        .AddSingleton<IPropertyAccessorProvider, JsonNodePropertyAccessorProvider>()
        .AddSingleton<IPropertyAccessorProvider, JsonDocumentPropertyAccessorProvider>()
        .AddSingleton<IPropertiesMatcher, PropertiesMatcher>()
        .AddSingleton<IPropertiesCache, PropertiesCache>()
        .AddSingleton<IInstanceFactory, InstanceFactory>()
        .AddSingleton<IMapBuilder, MapBuilder>()
        .AddSingleton<IDataMapper, DataMapperImplementation>()
        .AddSingleton<IAccessorCache, AccessorCache>()
        .AddSingleton<IPropertyAccessorFactory, PropertyAccessorFactory>()
        .AddSingleton<IPseudoPropertyAccessor, PseudoPropertyAccessor>();

    internal static T Get<T>() where T : notnull => ServiceProvider.GetRequiredService<T>();
}