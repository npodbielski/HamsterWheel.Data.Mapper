using System.Collections.ObjectModel;

namespace HamsterWheel.Data.Mapper.Maps;

public interface IMapBuilder
{
    ReadOnlyCollection<PropertyMapping> BuildMap(ObjectContext source, ObjectContext destination, Map map);
}