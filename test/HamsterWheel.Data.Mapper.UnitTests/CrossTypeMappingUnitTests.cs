// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable UnusedMember.Local

using FluentAssertions;

namespace HamsterWheel.Data.Mapper.UnitTests;

partial class DataMapperUnitTests
{
    private class Entity
    {
        public int Int { get; set; }
        public decimal Decimal { get; set; }
        public TimeOnly TimeOnly { get; set; }
        public TestEnum Enum { get; set; } = TestEnum.NotOK;

        public enum TestEnum
        {
            OK,
            NotOK
        }
    }

    [Fact]
    public void WhenMapsFromStringToInt_ThenCanMap()
    {
        //arrange
        var expected = new
        {
            Int = 1
        };
        var source = new
        {
            String = "1"
        };
        var destination = new Entity();

        //act
        DataMapper.Map(source, destination, [("String", "Int")]);

        //assert
        destination.Should().BeEquivalentTo(expected);
    }


    [Fact]
    public void WhenMapsFromStringToDecimal_ThenCanMap()
    {
        //arrange
        var expected = new
        {
            Decimal = 9.99m
        };
        var source = new
        {
            String = "9.99"
        };
        var destination = new Entity();

        //act
        DataMapper.Map(source, destination, [("String", "Decimal")]);

        //assert
        destination.Should().BeEquivalentTo(expected);
    }

    private class StringToDouble
    {
        public double Double { get; set; }
    }
    [Fact]
    public void WhenMapsFromStringToDouble_ThenCanMap()
    {
        //arrange
        var expected = new
        {
            Double = 9.99
        };
        var source = new
        {
            String = "9.99"
        };
        var destination = new StringToDouble();

        //act
        DataMapper.Map(source, destination, [("String", "Double")]);

        //assert
        destination.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void WhenMapsFromStringToEnum_ThenCanMap()
    {
        //arrange
        var expected = new
        {
            Enum = Entity.TestEnum.OK
        };
        var source = new
        {
            String = "OK"
        };
        var destination = new Entity();

        //act
        DataMapper.Map(source, destination, [("String", "Enum")]);

        //assert
        destination.Should().BeEquivalentTo(expected);
    }



    [Fact]
    public void WhenMapsFromDecimalToInteger_ThenCanMap()
    {
        //arrange
        var expected = new
        {
            Int = 10
        };
        var source = new
        {
            Decimal = 9.99m
        };
        var destination = new Entity();

        //act
        DataMapper.Map(source, destination, [("Decimal", "Int")]);

        //assert
        destination.Should().BeEquivalentTo(expected);
    }
    [Fact]
    public void WhenMapsFromDoubleToInteger_ThenCanMap()
    {
        //arrange
        var expected = new
        {
            Int = 10
        };
        var source = new
        {
            Double = 9.99
        };
        var destination = new Entity();

        //act
        DataMapper.Map(source, destination, [("Double", "Int")]);

        //assert
        destination.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void WhenMapsFromDateTimeToTimeOnly_ThenCanMap()
    {
        //arrange
        var expected = new
        {
            TimeOnly = new TimeOnly(10, 0, 0)
        };
        var source = new
        {
            DateTime = new DateTime(2023, 1, 1, 10, 0, 0, DateTimeKind.Utc).ToUniversalTime()
        };
        var destination = new Entity();

        //act
        DataMapper.Map(source, destination, [("DateTime", "TimeOnly")]);

        //assert
        destination.Should().BeEquivalentTo(expected);
    }
}