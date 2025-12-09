using HamsterWheel.Data.Mapper.Providers;

namespace HamsterWheel.Data.Mapper.Maps;

public record PropertyMapping(IPropertyAccessor Source, IPropertyAccessor Destination);