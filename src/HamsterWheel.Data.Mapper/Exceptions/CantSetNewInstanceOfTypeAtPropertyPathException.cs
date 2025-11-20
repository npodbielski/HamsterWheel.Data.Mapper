namespace HamsterWheel.Data.Mapper;

public class CantSetNewInstanceOfTypeAtPropertyPathException(string? propertyPath, Type type, Type declaringType) :
    DataMapperException(
        $"Can not set new instance of type '{type.FullName}' as property '{propertyPath}' of '{declaringType.FullName}' type. Most probable cause is private setter or compute only property.");