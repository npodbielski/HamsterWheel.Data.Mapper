using FluentAssertions;
using HamsterWheel.Data.Mapper.Setup;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace HamsterWheel.Data.Mapper.UnitTests.Readme;

public class ReadmeExampleUsageFromServices
{
    private class Source
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
    }

    private class Destination
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
    }

    [Fact]
    public void WhenMappingMultiplePropertiesOfTheSameNestedType_ThenCanMap()
    {
        //arrange
        var expected = new
        {
            FirstName = "Jan",
            LastName = "Kowalski",
        };
        var source = new Source
        {
            FirstName = "Jan",
            LastName = "Kowalski",
        };

        var destination = new Destination();
        var mapper = new ServiceCollection().ConfigureDataMapper().BuildServiceProvider()
            .GetRequiredService<IDataMapper>();

        //act
        mapper.Map(source, destination);

        //assert
        destination.Should().BeEquivalentTo(expected);
    }
}