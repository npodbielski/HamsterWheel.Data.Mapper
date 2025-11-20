namespace HamsterWheel.Data.Mapper;

public class MissingPropertyException(Type sourceType, string name)
    : DataMapperException($"No property: '{name}' found on type '{sourceType}'");