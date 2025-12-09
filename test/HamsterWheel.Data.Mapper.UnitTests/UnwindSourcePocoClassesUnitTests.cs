using System.Net;
using FluentAssertions;
using HamsterWheel.Data.Mapper.Maps;
using HamsterWheel.Data.Mapper.Setup;

namespace HamsterWheel.Data.Mapper.UnitTests;

public partial class MapBuilderUnitTests
{
    [Theory]
    [InlineData(typeof(bool), true)]
    [InlineData(typeof(bool?), true)]
    [InlineData(typeof(byte), true)]
    [InlineData(typeof(byte?), true)]
    [InlineData(typeof(char), true)]
    [InlineData(typeof(char?), true)]
    [InlineData(typeof(short), true)]
    [InlineData(typeof(short?), true)]
    [InlineData(typeof(ushort), true)]
    [InlineData(typeof(ushort?), true)]
    [InlineData(typeof(int), true)]
    [InlineData(typeof(int?), true)]
    [InlineData(typeof(nint), true)]
    [InlineData(typeof(nint?), true)]
    [InlineData(typeof(uint), true)]
    [InlineData(typeof(uint?), true)]
    [InlineData(typeof(long), true)]
    [InlineData(typeof(long?), true)]
    [InlineData(typeof(ulong), true)]
    [InlineData(typeof(ulong?), true)]
    [InlineData(typeof(float), true)]
    [InlineData(typeof(float?), true)]
    [InlineData(typeof(double), true)]
    [InlineData(typeof(double?), true)]
    [InlineData(typeof(decimal), true)]
    [InlineData(typeof(decimal?), true)]
    [InlineData(typeof(object), true)]
    [InlineData(typeof(void), true)]
    [InlineData(typeof(Delegate), true)]
    [InlineData(typeof(DateTime), true)]
    [InlineData(typeof(DateTimeOffset), true)]
    [InlineData(typeof(TimeOnly), true)]
    [InlineData(typeof(Uri), true)]
    [InlineData(typeof(Guid), true)]
    [InlineData(typeof(IPAddress), true)]
    [InlineData(typeof(DataMapperUnitTests.NestedObject), false)]
    [InlineData(typeof(Guid[]), false)]
    public void UnwindSourcePocoClasses_WhenCalled_ThenReturnsCorrectValue(Type type, bool shouldBeNull)
    {
        //arrange
        var chunk = new PropertyPathChunk("*");
        var propertyAccessors = new PropertyAccessor(typeof(object), type, PropertyPath.From("a"), o => o, (o, o1) => {});

        //act
        var actual = ((MapBuilder)Services.Get<IMapBuilder>()).UnwindSourcePocoClasses(chunk, propertyAccessors);

        //assert
        if (shouldBeNull)
        {
            actual.Should().BeNull();
        }
        else
        {
            actual.Should().NotBeEmpty();
        }
    }
}