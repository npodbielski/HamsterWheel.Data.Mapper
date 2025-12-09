using FluentAssertions;

// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace HamsterWheel.Data.Mapper.UnitTests.Readme;

public class ReadmeExample6UnitTest
{
    private class Source
    {
        public string FullName { get; set; } = null!;
    }

    private class Destination
    {
        public string Name { get; set; } = null!;
    }

    [Fact]
    public void WhenMapAndIncompatibleProperties_ThenCanMap()
    {
        //arrange
        var expected = new
        {
            Name = ""
        };
        var source = new Source
        {
            FullName = "Kowalski"
        };

        var destination = new Destination
        {
            Name = ""
        };

        //act
        DataMapper.Map(source, destination);

        //assert
        destination.Should().BeEquivalentTo(expected);
    }
}