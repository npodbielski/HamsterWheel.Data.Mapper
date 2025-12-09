using FluentAssertions;

// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace HamsterWheel.Data.Mapper.UnitTests.Readme;

public class ReadmeExample15UnitTest
{
    private class Source
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public Address UserResidenceAddress { get; set; } = null!;
        public Address UserWorkAddress { get; set; } = null!;
    }

    private class Destination
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public Address UserResidenceAddress { get; set; } = null!;
        public Address UserWorkAddress { get; set; } = null!;
    }

    private class Address
    {
        public string Street { get; set; } = null!;
        public string City { get; set; } = null!;
    }

    [Fact]
    public void WhenMappingMultiplePropertiesOfTheSameNestedType_ThenCanMap()
    {
        //arrange
        var expected = new
        {
            FirstName = (string?)null,
            LastName = (string?)null,
            UserResidenceAddress = new { Street = "ResidenceStreet", City = "ResidenceCity" },
            UserWorkAddress = new { Street = "OtherStreet", City = "SomeCity" },
        };
        var source = new Source
        {
            FirstName = "Jan",
            LastName = "Kowalski",
            UserResidenceAddress = new() { Street = "ResidenceStreet", City = "ResidenceCity" },
            UserWorkAddress = new() { Street = "OtherStreet", City = "SomeCity" },
        };

        var destination = new Destination();

        //act
        DataMapper.Map(source, destination, [("*", "User*Address")]);

        //assert
        destination.Should().BeEquivalentTo(expected);
    }
}