using FluentAssertions;

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace HamsterWheel.Data.Mapper.UnitTests.Readme;

public class ReadmeExample13UnitTest
{
    private class UserWithAddress
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Street { get; set; } = null!;
        public string City { get; set; } = null!;
    }

    private class User
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public Address Address { get; set; } = null!;
    }

    private class Address
    {
        public string Street { get; set; } = null!;
        public string City { get; set; } = null!;
    }

    [Fact]
    public void WhenFlatteningNestedObjects_ThenCanMap()
    {
        //arrange
        var expected = new
        {
            FirstName = "Jan",
            LastName = "Kowalski",
            Street = "Marszałkowska",
            City = "Warszawa"
        };
        var source = new User
        {
            FirstName = "Jan",
            LastName = "Kowalski",
            Address = new()
            {
                Street = "Marszałkowska",
                City = "Warszawa"
            }
        };

        var destination = new UserWithAddress();

        //act
        DataMapper.Map(source, destination, [("*", "*"), ("Address.*", "*")]);

        //assert
        destination.Should().BeEquivalentTo(expected);
    }
}