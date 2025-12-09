using FluentAssertions;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace HamsterWheel.Data.Mapper.UnitTests.Readme;

public class ReadmeExample11UnitTest
{
    private class Source
    {
        public string Name { get; set; } = null!;
        public string PropString { get; set; } = null!;
        public int PropInt { get; set; }
        public bool PropBool { get; set; }
        public byte PropByte { get; set; }
    }

    private class Destination
    {
        public string Name { get; set; } = null!;
        public string PropString { get; set; } = null!;
        public int PropInt { get; set; }
        public bool PropBool { get; set; }
        public byte PropByte { get; set; }
    }

    [Fact]
    public void WhenMapAndIncompatibleProperties_ThenCanMap()
    {
        //arrange
        var expected = new
        {
            Name = "Smith",
            PropString = "Test",
            PropInt = 132320,
            PropBool = true,
            PropByte = 10
        };
        var source = new Source
        {
            Name = "Jan Kowalski",
            PropString = "Test",
            PropInt = 132320,
            PropBool = true,
            PropByte = 10
        };

        var destination = new Destination { Name = "Smith" };

        //act
        DataMapper.Map(source, destination, [("Prop*", "Prop*")]);

        //assert
        destination.Should().BeEquivalentTo(expected);
    }
}