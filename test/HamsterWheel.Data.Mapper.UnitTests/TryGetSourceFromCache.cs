using FluentAssertions;
using HamsterWheel.Data.Mapper.Maps;
using HamsterWheel.Data.Mapper.Providers;
using HamsterWheel.Data.Mapper.Setup;
using HamsterWheel.HLinq.Reflection;

namespace HamsterWheel.Data.Mapper.UnitTests;

partial class PropertyAccessorFactoryUnitTests
{
    private readonly PropertyAccessorFactory _sut = new(new PropertiesCache(), new InstanceFactory(),
        new AccessorCache(),
        Services.Get<IPseudoPropertyAccessor>());

    [Fact]
    public void TryGetFromCache_WhenCalledWithWildCard_ThenThrowsException()
    {
        //arrange
        var wildcardChunk = new PropertyPathChunk("*");
        var action = () => _sut.TryGetFromCache(typeof(object), wildcardChunk);

        //act
        var exception = action.Should().Throw<FetchingFromCacheWithWildcardException>();

        //assert
        exception.WithMessage("No cached data for wildcard");
    }
}