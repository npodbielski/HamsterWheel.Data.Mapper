using HamsterWheel.Exceptions;

namespace HamsterWheel.Data.Mapper;

[PlatformException]
public abstract class DataMapperException(string message) : InternalPlatformException(message);