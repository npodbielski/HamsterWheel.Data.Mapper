using FluentAssertions;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable UnusedMember.Local

namespace HamsterWheel.Data.Mapper.UnitTests.Readme;

public class ReadmeExample12UnitTest
{
    private class UserWithAddress
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string AddressStreet { get; set; } = null!;
        public string AddressCity { get; set; } = null!;
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
            AddressStreet = "Marszałkowska",
            AddressCity = "Warszawa"
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
        DataMapper.Map(source, destination,
            [("*", "*"), ("Address.City", "AddressCity"), ("Address.Street", "AddressStreet")]);

        //assert
        destination.Should().BeEquivalentTo(expected);
    }
}