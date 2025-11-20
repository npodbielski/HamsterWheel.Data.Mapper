namespace HamsterWheel.Data.Mapper;

public class CreationOfTypeFailedException(Type type)
    : DataMapperException("Could not create instance of type: " + type.FullName);