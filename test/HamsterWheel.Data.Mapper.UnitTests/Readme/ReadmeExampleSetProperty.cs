using FluentAssertions;
using HamsterWheel.Data.Mapper.Setup;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace HamsterWheel.Data.Mapper.UnitTests.Readme;

public class ReadmeExampleSetProperty
{
    private class Destination
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
    }

    [Fact]
    public void WhenMapIsSetPropertyMap_ThenCanMap()
    {
        //arrange
        var expected = new
        {
            FirstName = "Jan",
            LastName = "Kowalski",
        };
        var source = "Jan";
        var destination = new Destination
        {
            LastName = "Kowalski"
        };
        var mapper = new ServiceCollection().ConfigureDataMapper().BuildServiceProvider()
            .GetRequiredService<IDataMapper>();

        //act
        mapper.Map(source, destination, [(".", "FirstName")]);

        //assert
        destination.Should().BeEquivalentTo(expected);
    }
}