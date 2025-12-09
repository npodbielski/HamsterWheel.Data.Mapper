using FluentAssertions;
using HamsterWheel.Data.Mapper.Maps;

namespace HamsterWheel.Data.Mapper.UnitTests;

public partial class PropertyAccessorFactoryUnitTests
{
    [Fact]
    public void Setter_WhenDestinationNestedPropertyHaveIntermediatePropReadOnly_ThenThrowsException()
    {
        //arrange
        var accessor = new PropertyAccessor(typeof(object), typeof(object), PropertyPath.From("a"), _ => null, null);
        var destProp = _sut.ToDestProp(accessor, new PropertyAccessor(null, typeof(object), PropertyPath.From("a.b"),
            null, (_, _) => { }));
        var action = () => destProp.Setter!(new { a = new { b = 1 } }, 2);

        //act && assert
        action.Should().Throw<PropertyAccessorFactory.CantSetNewInstanceOfTypeAtPropertyPathException>();
    }

    [Fact]
    public void Create_WhenCalledWithMustHaveGetterTrueAndPropertyIsSetOnly_ThenThrowsException()
    {
        //arrange
        var type = typeof(DataMapperUnitTests.WriteOnlyPropertyClass);
        var propName = nameof(DataMapperUnitTests.WriteOnlyPropertyClass.Test);
        var action = () => _sut.Create(type, new PropertyPathChunk(propName), mustHaveGetter: true);

        //act
        var exception = action.Should().Throw<MissingPropertyGetterException>();

        //assert
        exception.WithMessage($"*{propName}' does not have a getter");
    }
}