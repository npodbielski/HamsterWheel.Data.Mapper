using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace HamsterWheel.Data.Mapper;

public static class TypeExtensions
{
    public static List<Type> ExcludeTypes { get; } = [typeof(string), typeof(Uri), typeof(object), typeof(Delegate), typeof(IPAddress)];
    
    public static bool IsCustomType([NotNullWhen(true)]this Type? propertyType) =>
        propertyType is { IsPrimitive: false, IsValueType: false }
        && !ExcludeTypes.Contains(propertyType);
}