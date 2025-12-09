using FluentAssertions;
using HamsterWheel.Data.Mapper.Maps;

namespace HamsterWheel.Data.Mapper.UnitTests;

partial class PropertyAccessorFactoryUnitTests
{
    [Fact]
    public void ToSourceProp_WhenCalledAndGetterIsNull_ThenThrowsException()
    {
        //arrange
        var accessor = new PropertyAccessor(null, null!, new PropertyPath([new PropertyPathChunk("foo")]), null, null);
        var action = () =>
            _sut.ToSourceProp(accessor, new PropertyAccessor(null, null!,
                new PropertyPath([new PropertyPathChunk("bar")]), null,
                null));

        //act
        var exception = action.Should().Throw<PropertyAccessorFactory.RootSourcePropertyGetterNullException>();

        //assert
        exception.WithMessage("Cannot create nested source property if root property getter is null");
    }

    [Fact]
    public void ToSourceProp_WhenCalledAndIntermediatePropGetterIsNull_ThenThrowsException()
    {
        //arrange
        var accessor = new PropertyAccessor(null, null!, new PropertyPath([new PropertyPathChunk("foo")]), o => o, null);
        var action = () =>
            _sut.ToSourceProp(accessor, new PropertyAccessor(null, null!,
                new PropertyPath([new PropertyPathChunk("bar")]), null,
                null));

        //act
        var exception = action.Should().Throw<PropertyAccessorFactory.IntermediateSourcePropertyGetterNullException>();

        //assert
        exception.WithMessage("Cannot create nested source property if intermediate getter is null");
    }
}