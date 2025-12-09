using FluentAssertions;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable UnusedMember.Local

namespace HamsterWheel.Data.Mapper.UnitTests.Readme;

public class ReadmeExample1UnitTest
{
    private class Source
    {
        public string FirstName { get; set; }= null!;
        public string LastName { get; set; }= null!;
        public int Age { get; set; }
    }

    private class Destination
    {
        public string LastName { get; set; }= null!;
        public int Age { get; set; }
    }

    [Fact]
    public void WhenNoMapAndNoConversion_ThenCanMap()
    {
        //arrange
        var expected = new
        {
            LastName = "Kowalski",
            Age = 25
        };
        var source = new Source
        {
            FirstName = "Jan",
            LastName = "Kowalski",
            Age = 25
        };
        var destination = new Destination();

        //act
        DataMapper.Map(source, destination);

        //assert
        destination.Should().BeEquivalentTo(expected);
    }
}