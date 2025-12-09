namespace HamsterWheel.Data.Mapper;

public class MissingPropertyGetterException(string? propPath)
    : DataMapperException($"Property: '{propPath}' does not have a getter");