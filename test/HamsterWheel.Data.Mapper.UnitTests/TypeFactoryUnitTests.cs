using FluentAssertions;
using HamsterWheel.Data.Mapper.Providers;

namespace HamsterWheel.Data.Mapper.UnitTests;

public class InstanceFactoryUnitTests
{
    [Fact]
    public void Create_When_Then()
    {
        //arrange
        int? expected = null;

        //act
        var actual = new InstanceFactory().Create(typeof(int?));

        //assert
        actual.Should().Be(expected);
    }
}