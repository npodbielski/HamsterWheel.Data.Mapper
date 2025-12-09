using FluentAssertions;

// ReSharper disable ClassNeverInstantiated.Local
// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace HamsterWheel.Data.Mapper.UnitTests.Readme;

public class ReadmeExample5UnitTest
{
    private class Source
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public AddressData Address { get; set; } = null!;
    }

    private class AddressData
    {
        public string Street { get; set; } = null!;
        public string City { get; set; } = null!;
        public ContactData Contact { get; set; } = null!;
    }

    private class ContactData
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
    }

    private class Destination
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public DestinationAddress Address { get; set; } = null!;
    }

    private class DestinationAddress
    {
        public string Street { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Country { get; set; } = null!;
        public PersonOfContact Contact { get; set; } = null!;
    }

    private class PersonOfContact
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
    }

    [Fact]
    public void WhenNoMapAndAnd3rdNestedObject_ThenCanMapWithoutClearingExistingValues()
    {
        //arrange
        var expected = new
        {
            FirstName = "Jan",
            LastName = "Kowalski",
            Address = new
            {
                Street = "SomeStreet",
                City = "SomeCity",
                Country = "Poland",
                Contact = new
                {
                    FirstName = "John",
                    LastName = "Kowalski",
                }
            }
        };
        var source = new Source
        {
            FirstName = "Jan",
            LastName = "Kowalski",
            Address = new AddressData
            {
                Street = "SomeStreet",
                City = "SomeCity",
                Contact = new ContactData
                {
                    FirstName = "John",
                    LastName = "Kowalski",
                }
            }
        };

        var destination = new Destination
        {
            Address = new DestinationAddress
            {
                Country = "Poland"
            }
        };

        //act
        DataMapper.Map(source, destination);

        //assert
        destination.Should().BeEquivalentTo(expected);
    }
}