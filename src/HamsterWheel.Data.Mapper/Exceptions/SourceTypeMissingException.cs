namespace HamsterWheel.Data.Mapper;

public class SourceTypeMissingException()
    : DataMapperException("For map that is not property set: '.' source object need to be provided.");