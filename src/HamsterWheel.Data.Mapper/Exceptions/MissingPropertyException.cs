namespace HamsterWheel.Data.Mapper;

public class MissingPropertyException(Type sourceType, string name)
    : DataMapperException($"No property or index: '{name}' found on type '{sourceType}'");