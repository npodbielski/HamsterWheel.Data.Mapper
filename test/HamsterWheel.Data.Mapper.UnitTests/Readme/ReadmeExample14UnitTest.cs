using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using HamsterWheel.Data.Mapper.Maps;
using NSubstitute;
// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable UnusedMember.Local

namespace HamsterWheel.Data.Mapper.UnitTests.Readme;

public class ReadmeExample14UnitTest
{
    private class Source
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Age { get; set; } = null!;
    }

    private class Destination
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public int Age { get; set; }
    }

    [Fact]
    [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
    [SuppressMessage("ReSharper",
        "GenericEnumeratorNotDisposed")] //point of test is to substitute the map via IEnumerable
    public void WhenReusingMap_ThenCanMapAndDoNotCreateSecondMap()
    {
        //arrange
        var expected = new
        {
            FirstName = "Jan",
            LastName = "Kowalski",
            Age = 23
        };
        var source = new Source
        {
            FirstName = "Jan",
            LastName = "Kowalski",
            Age = "23"
        };

        var destination = new Destination();
        var cachedMap = DataMapper.BuildMap<Source, Destination>();
        var substituteMap = Substitute.For<IEnumerable<PropertyMapping>>();
        substituteMap.GetEnumerator().Returns(cachedMap.GetEnumerator());

        //act
        DataMapper.Map(source, destination, cachedMap: substituteMap);

        //assert
        destination.Should().BeEquivalentTo(expected);
        substituteMap.Received(1).GetEnumerator().Dispose();
    }
}