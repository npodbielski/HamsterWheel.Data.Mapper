using FluentAssertions;

namespace HamsterWheel.Data.Mapper.UnitTests.Readme;

public class ReadmeExample10UnitTest
{
    public class Source
    {
        public string FullName { get; set; } = null!;
        public int Age { get; set; }
        public bool Active { get; set; }
    }

    public class Destination
    {
        public string Name { get; set; } = null!;
        public int Age { get; set; }
        public bool Active { get; set; }
    }

    [Fact]
    public void WhenMapAndIncompatibleProperties_ThenCanMap()
    {
        //arrange
        var expected = new
        {
            Name = "Jan Kowalski",
            Age = 30,
            Active = true
        };
        var source = new Source
        {
            FullName = "Jan Kowalski",
            Age = 30,
            Active = true
        };

        var destination = new Destination
        {
            Name = ""
        };

        //act
        DataMapper.Map(source, destination, [("*", "*"), ("FullName", "Name")]);

        //assert
        destination.Should().BeEquivalentTo(expected);
    }
}