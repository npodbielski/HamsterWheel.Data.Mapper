using FluentAssertions;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace HamsterWheel.Data.Mapper.UnitTests.Readme;

public class ReadmeExample2UnitTest
{
    private class Source
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Active { get; set; } = null!;
        public int Age { get; set; }
    }

    private class Destination
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public UserStatus Active { get; set; }
        public string Age { get; set; } = null!;
    }

    public enum UserStatus
    {
        Active,
        Inactive
    }

    [Fact]
    public void WhenNoMapAndNeedConversion_ThenCanMap()
    {
        //arrange
        var expected = new
        {
            FirstName = "Jan",
            LastName = "Kowalski",
            Age = "25",
            Active = UserStatus.Active
        };
        var source = new Source
        {
            FirstName = "Jan",
            LastName = "Kowalski",
            Active = "Active",
            Age = 25
        };
        var destination = new Destination();

        //act
        DataMapper.Map(source, destination);

        //assert
        destination.Should().BeEquivalentTo(expected);
    }
}