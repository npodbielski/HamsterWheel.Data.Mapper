using FluentAssertions;
using HamsterWheel.Data.Mapper.Maps;

namespace HamsterWheel.Data.Mapper.UnitTests;

partial class PropertyAccessorFactoryUnitTests
{
    [Fact]
    public void ToDestProp_WhenIntermediatePropertySetterIsNull_ThenThrowsException()
    {
        //arrange
        var accessor = new PropertyAccessor(null, null!, PropertyPath.From("a"), o => o, null);
        var action = () =>
            _sut.ToDestProp(accessor, new PropertyAccessor(null, null!, PropertyPath.From("a.b"), null, null));

        //act && assert
        action.Should().Throw<PropertyAccessorFactory.IntermediateDestinationPropertySetterNullException>();
    }

    [Fact]
    public void ToDestProp_WhenParentPropGetterIsNull_ThenThrowsException()
    {
        //arrange
        var accessor = new PropertyAccessor(null, null!, PropertyPath.From("a"), null, null);
        var action = () =>
            _sut.ToDestProp(accessor, new PropertyAccessor(null, null!, PropertyPath.From("a.b"), null, null));

        //act && assert
        action.Should().Throw<PropertyAccessorFactory.RootDestinationPropertyGetterNullException>();
    }
}