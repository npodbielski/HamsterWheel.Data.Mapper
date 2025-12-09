namespace HamsterWheel.Data.Mapper;

public class MissingPropertySetterException(string? propPath)
    : DataMapperException($"Property: '{propPath}' does not have a setter");