using System.Diagnostics.CodeAnalysis;

namespace HamsterWheel.Data.Mapper;

//This exception will only be thrown if the InstanceFactory is called with Nullable<T> type, but in that case there is a quick path in InstanceFactory. Therefore, this type will never make to the Activator. In theory this exception should never be thrown.
[ExcludeFromCodeCoverage]
public class CreationOfTypeFailedException(Type type)
    : DataMapperException($"Could not create instance of type: {type.FullName}");