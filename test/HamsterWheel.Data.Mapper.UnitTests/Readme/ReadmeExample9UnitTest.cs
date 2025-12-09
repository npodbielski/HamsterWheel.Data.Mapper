using FluentAssertions;

// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace HamsterWheel.Data.Mapper.UnitTests.Readme;

public class ReadmeExample9UnitTest
{
    private class Source
    {
        public string FirsName { get; set; } = null!;
        public string LastName { get; set; } = null!;
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
            FirsName = "Jan",
            LastName = "Kowalski"
        };

        var destination = new Destination
        {
            Name = ""
        };

        //act
        DataMapper.Map(source, destination, [("*Name", "Name")]);

        //assert
        destination.Should().BeEquivalentTo(expected);
    }
}