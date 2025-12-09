using System.Text.Json;
using System.Text.Json.Nodes;
using FluentAssertions;
using HamsterWheel.Data.Mapper.Maps;

namespace HamsterWheel.Data.Mapper.UnitTests;

using static DataMapper;

partial class MapBuilderUnitTests
{
    [Fact]
    public void BuildMap_WhenTwoStarsAndMatchingTypes_ThenIncludeAllProperties()
    {
        //arrange
        Map expected = [("Prop2", "Prop2"), ("Prop3", "Prop3")];
        var destination = new DataMapperUnitTests.NestedObject();
        var source = new DataMapperUnitTests.NestedObject();

        //act
        var map = BuildMap(source, destination);

        //assert
        map.Should().HavePaths(expected);
    }

    [Fact]
    public void BuildMap_WhenWithTwoTypes_ThenReturnsCorrectMap()
    {
        //arrange
        Map expected = [("Prop2", "Prop2"), ("Prop3", "Prop3")];

        //act
#pragma warning disable CA2263 //whole point of tests
        var map = BuildMap(typeof(DataMapperUnitTests.NestedObject), typeof(DataMapperUnitTests.FlatHierarchyClass));
#pragma warning restore CA2263

        //assert
        map.Should().HavePaths(expected);
    }

    [Fact]
    public void BuildMap_WhenTwoStarsAndMatchingTypesAndNoMapPassed_ThenIncludeAllProperties()
    {
        //arrange
        Map expected = [("Prop2", "Prop2"), ("Prop3", "Prop3")];
        var destination = new DataMapperUnitTests.NestedObject();
        var source = new DataMapperUnitTests.NestedObject();

        //act
        var map = BuildMap(source, destination, AnyToAnyMap);

        //assert
        map.Should().HavePaths(expected);
    }

    [Fact]
    public void BuildMap_WhenTwoStarsAndDifferentTypes_ThenIncludeMatchingProperties()
    {
        //arrange
        Map expected = [("Prop2", "Prop2"), ("Prop3", "Prop3")];
        var destination = new DataMapperUnitTests.FlatHierarchyClass();
        var source = new DataMapperUnitTests.NestedObject();

        //act
        var map = BuildMap(source, destination, AnyToAnyMap);

        //assert
        map.Should().HavePaths(expected);
    }

    [Fact]
    public void BuildMap_WhenTwoStarsAndDifferentTypesWithoutMatchingProps_ThenReturnsEmpty()
    {
        //arrange
        var destination = new DataMapperUnitTests.DummyTokenInfo();
        var source = new DataMapperUnitTests.NestedObject();

        //act
        var map = BuildMap(source, destination, AnyToAnyMap);

        //assert
        map.Should().BeEmpty();
    }

    [Fact]
    public void
        BuildMap_WhenTwoStarsAndDifferentTypesWithMatchingPropsButOfDifferentTypes_ThenReturnsMatchingProperties()
    {
        //arrange
        Map expected = [("Key", "Key")];
        var destination = new DataMapperUnitTests.GuidKeyEntity();
        var source = new DataMapperUnitTests.StringKeyEntity();

        //act
        var map = BuildMap(source, destination, AnyToAnyMap);

        //assert
        map.Should().HavePaths(expected);
    }

    [Fact]
    public void BuildMap_WhenSourceStarAndDestinationDirectProperty_ThenReturnsMatchingProperties()
    {
        //arrange
        Map expected = [("Key", "Nested.Key")];
        var destination =
            new DataMapperUnitTests.DirectRoot<DataMapperUnitTests.GuidKeyEntity>(
                new DataMapperUnitTests.GuidKeyEntity());
        var source = new DataMapperUnitTests.StringKeyEntity();

        //act
        var map = BuildMap(source, destination, [(AnyProperty, "Nested")]);

        //assert
        map.Should().HavePaths(expected);
    }

    [Fact]
    public void BuildMap_WhenSourceDirectPropertyAndDestinationStar_ThenReturnsMatchingProperties()
    {
        //arrange
        Map expected = [("Nested.Key", "Key")];
        var destination = new DataMapperUnitTests.StringKeyEntity();
        var source =
            new DataMapperUnitTests.DirectRoot<DataMapperUnitTests.GuidKeyEntity>(
                new DataMapperUnitTests.GuidKeyEntity());

        //act
        var map = BuildMap(source, destination, [("Nested", AnyProperty)]);

        //assert
        map.Should().HavePaths(expected);
    }

    [Fact]
    public void BuildMap_WhenSourceDirectPropertyAndDestinationDirectProperty_ThenReturnsThoseProperties()
    {
        //arrange
        Map expected = [("Nested", "Nested")];
        var destination =
            new DataMapperUnitTests.DirectRoot<DataMapperUnitTests.StringKeyEntity>(
                new DataMapperUnitTests.StringKeyEntity());
        var source =
            new DataMapperUnitTests.DirectRoot<DataMapperUnitTests.GuidKeyEntity>(
                new DataMapperUnitTests.GuidKeyEntity());

        //act
        var map = BuildMap(source, destination, [("Nested", "Nested")]);

        //assert
        map.Should().HavePaths(expected);
    }

    [Fact]
    public void BuildMap_WhenSourceNestedWithStarAndDestinationDirectProperty_ThenReturnsAnySubMatchingProperties()
    {
        //arrange
        Map expected = [("Nested.Key", "Nested.Key")];
        var destination =
            new DataMapperUnitTests.DirectRoot<DataMapperUnitTests.StringKeyEntity>(
                new DataMapperUnitTests.StringKeyEntity());
        var source =
            new DataMapperUnitTests.DirectRoot<DataMapperUnitTests.GuidKeyEntity>(
                new DataMapperUnitTests.GuidKeyEntity());

        //act
        var map = BuildMap(source, destination, [("Nested.*", "Nested")]);

        //assert
        map.Should().HavePaths(expected);
    }

    [Fact]
    public void BuildMap_WhenSourceDirectPropertyAndDestinationNestedWithStar_ThenReturnsAnySubMatchingProperties()
    {
        //arrange
        Map expected = [("Nested.Key", "Nested.Key")];
        var destination =
            new DataMapperUnitTests.DirectRoot<DataMapperUnitTests.StringKeyEntity>(
                new DataMapperUnitTests.StringKeyEntity());
        var source =
            new DataMapperUnitTests.DirectRoot<DataMapperUnitTests.GuidKeyEntity>(
                new DataMapperUnitTests.GuidKeyEntity());

        //act
        var map = BuildMap(source, destination, [("Nested", "Nested.*")]);

        //assert
        map.Should().HavePaths(expected);
    }

    [Fact]
    public void BuildMap_WhenSourceDeeplyNestedPropertyAndDestinationDeeplyNested_ThenReturnsAnySubMatchingProperties()
    {
        //arrange
        Map expected = [("DeeplyNested.Nested", "DeeplyNested.Nested")];
        var source =
            new DataMapperUnitTests.DeeplyNestedRoot<DataMapperUnitTests.GuidKeyEntity>(
                new DataMapperUnitTests.GuidKeyEntity());
        var destination =
            new DataMapperUnitTests.DeeplyNestedRoot<DataMapperUnitTests.StringKeyEntity>(
                new DataMapperUnitTests.StringKeyEntity());

        //act
        var map = BuildMap(source, destination, [("DeeplyNested.Nested", "DeeplyNested.Nested")]);

        //assert
        map.Should().HavePaths(expected);
    }

    [Fact]
    public void BuildMap_WhenSourceDirectPropertyAndDestinationDeeplyNested_ThenReturnsAnySubMatchingProperties()
    {
        //arrange
        Map expected = [("Nested", "DeeplyNested.Nested")];
        var source =
            new DataMapperUnitTests.DirectRoot<DataMapperUnitTests.StringKeyEntity>(
                new DataMapperUnitTests.StringKeyEntity());
        var destination =
            new DataMapperUnitTests.DeeplyNestedRoot<DataMapperUnitTests.GuidKeyEntity>(
                new DataMapperUnitTests.GuidKeyEntity());

        //act
        var map = BuildMap(source, destination, [("Nested", "DeeplyNested.Nested")]);

        //assert
        map.Should().HavePaths(expected);
    }

    [Fact]
    public void BuildMap_WhenSourceDeeplyNestedPropertyAndDestinationDirect_ThenReturnsAnySubMatchingProperties()
    {
        //arrange
        Map expected = [("DeeplyNested.Nested", "Nested")];
        var source =
            new DataMapperUnitTests.DeeplyNestedRoot<DataMapperUnitTests.StringKeyEntity>(
                new DataMapperUnitTests.StringKeyEntity());
        var destination =
            new DataMapperUnitTests.DirectRoot<DataMapperUnitTests.GuidKeyEntity>(
                new DataMapperUnitTests.GuidKeyEntity());

        //act
        var map = BuildMap(source, destination, [("DeeplyNested.Nested", "Nested")]);

        //assert
        map.Should().HavePaths(expected);
    }

    [Fact]
    public void
        BuildMap_WhenSourceDeeplyNestedWithStarPropertyAndDestinationDeeplyNested_ThenReturnsAnySubMatchingProperties()
    {
        //arrange
        Map expected = [("DeeplyNested.Nested.Key", "DeeplyNested.Nested.Key")];
        var source =
            new DataMapperUnitTests.DeeplyNestedRoot<DataMapperUnitTests.GuidKeyEntity>(
                new DataMapperUnitTests.GuidKeyEntity());
        var destination =
            new DataMapperUnitTests.DeeplyNestedRoot<DataMapperUnitTests.StringKeyEntity>(
                new DataMapperUnitTests.StringKeyEntity());

        //act
        var map = BuildMap(source, destination, [("DeeplyNested.Nested.*", "DeeplyNested.Nested")]);

        //assert
        map.Should().HavePaths(expected);
    }

    [Fact]
    public void
        BuildMap_WhenSourceDirectPropertyWithStarAndDestinationDeeplyNested_ThenReturnsAnySubMatchingProperties()
    {
        //arrange
        Map expected = [("Nested.Key", "DeeplyNested.Nested.Key")];
        var destination =
            new DataMapperUnitTests.DeeplyNestedRoot<DataMapperUnitTests.StringKeyEntity>(
                new DataMapperUnitTests.StringKeyEntity());
        var source =
            new DataMapperUnitTests.DirectRoot<DataMapperUnitTests.GuidKeyEntity>(
                new DataMapperUnitTests.GuidKeyEntity());

        //act
        var map = BuildMap(source, destination, [("Nested.*", "DeeplyNested.Nested")]);

        //assert
        map.Should().HavePaths(expected);
    }

    [Fact]
    public void
        BuildMap_WhenSourceDeeplyNestedWithStarPropertyAndDestinationDirect_ThenReturnsAnySubMatchingProperties()
    {
        //arrange
        Map expected = [("DeeplyNested.Nested.Key", "Nested.Key")];
        var source =
            new DataMapperUnitTests.DeeplyNestedRoot<DataMapperUnitTests.GuidKeyEntity>(
                new DataMapperUnitTests.GuidKeyEntity());
        var destination =
            new DataMapperUnitTests.DirectRoot<DataMapperUnitTests.StringKeyEntity>(
                new DataMapperUnitTests.StringKeyEntity());

        //act
        var map = BuildMap(source, destination, [("DeeplyNested.Nested.*", "Nested")]);

        //assert
        map.Should().HavePaths(expected);
    }

    [Fact]
    public void
        BuildMap_WhenSourceDeeplyNestedPropertyAndDestinationDeeplyNestedWithStar_ThenReturnsAnySubMatchingProperties()
    {
        //arrange
        Map expected = [("DeeplyNested.Nested.Key", "DeeplyNested.Nested.Key")];
        var source =
            new DataMapperUnitTests.DeeplyNestedRoot<DataMapperUnitTests.GuidKeyEntity>(
                new DataMapperUnitTests.GuidKeyEntity());
        var destination =
            new DataMapperUnitTests.DeeplyNestedRoot<DataMapperUnitTests.StringKeyEntity>(
                new DataMapperUnitTests.StringKeyEntity());

        //act
        var map = BuildMap(source, destination, [("DeeplyNested.Nested", "DeeplyNested.Nested.*")]);

        //assert
        map.Should().HavePaths(expected);
    }

    [Fact]
    public void
        BuildMap_WhenSourceDirectPropertyAndDestinationDeeplyNestedWithStar_ThenReturnsAnySubMatchingProperties()
    {
        //arrange
        Map expected = [("Nested.Key", "DeeplyNested.Nested.Key")];
        var destination =
            new DataMapperUnitTests.DeeplyNestedRoot<DataMapperUnitTests.StringKeyEntity>(
                new DataMapperUnitTests.StringKeyEntity());
        var source =
            new DataMapperUnitTests.DirectRoot<DataMapperUnitTests.GuidKeyEntity>(
                new DataMapperUnitTests.GuidKeyEntity());

        //act
        var map = BuildMap(source, destination, [("Nested", "DeeplyNested.Nested.*")]);

        //assert
        map.Should().HavePaths(expected);
    }

    [Fact]
    public void
        BuildMap_WhenSourceDeeplyNestedPropertyAndDestinationDirectWithStar_ThenReturnsAnySubMatchingProperties()
    {
        //arrange
        Map expected = [("DeeplyNested.Nested.Key", "Nested.Key")];
        var source =
            new DataMapperUnitTests.DeeplyNestedRoot<DataMapperUnitTests.GuidKeyEntity>(
                new DataMapperUnitTests.GuidKeyEntity());
        var destination =
            new DataMapperUnitTests.DirectRoot<DataMapperUnitTests.StringKeyEntity>(
                new DataMapperUnitTests.StringKeyEntity());

        //act
        var map = BuildMap(source, destination, [("DeeplyNested.Nested", "Nested.*")]);

        //assert
        map.Should().HavePaths(expected);
    }

    [Fact]
    public void BuildMap_WhenDotAsSourceProperty_ThenMapsSingleProperty()
    {
        //arrange
        Map expected = [(".", "Prop2")];
        var destination = new DataMapperUnitTests.NestedObject();
        var source = 1;

        //act
        var map = BuildMap(source, destination, expected);

        //assert
        map.Should().HavePaths(expected);
    }

    [Fact]
    public void BuildMap_WhenSourceWildcardDoesNotHaveMatchingProp_ThenEmptyMapReturned()
    {
        //arrange
        Map requestedMap = [("Test*", "Prop2")];
        var destination = new DataMapperUnitTests.NestedObject();
        var source = new { Hello = "World" };

        //act
        var compiledMap = BuildMap(source, destination, requestedMap);

        //assert
        compiledMap.Should().HavePaths([]);
    }

    [Fact]
    public void BuildMap_WhenWildcardAtTheStart_ThenReturnsMatchingProperties()
    {
        //arrange
        Map requestedMap = [("*llo", "Prop2")];
        var destination = new DataMapperUnitTests.NestedObject();
        var source = new { Hello = "World" };

        //act
        var compiledMap = BuildMap(source, destination, requestedMap);

        //assert
        compiledMap.Should().HavePaths([("Hello", "Prop2")]);
    }

    [Fact]
    public void BuildMap_WhenWildcardInTheMiddle_ThenReturnsMatchingProperties()
    {
        //arrange
        Map requestedMap = [("H*llo", "Prop2")];
        var destination = new DataMapperUnitTests.NestedObject();
        var source = new { Hello = "World" };

        //act
        var compiledMap = BuildMap(source, destination, requestedMap);

        //assert
        compiledMap.Should().HavePaths([("Hello", "Prop2")]);
    }

    [Fact]
    public void BuildMap_WhenPropertiesCacheReturnsNull_ThenThrowsException()
    {
        //arrange
        var destination = new DataMapperUnitTests.NestedObject();
        var source = new { Hello = "World" };
        var action = () => BuildMap(source, destination, [("Hello", "World")]);

        //act
        var exception = action.Should().Throw<MissingPropertyException>();

        //assert
        exception.WithMessage(
            $"No property or index: 'World' found on type '{typeof(DataMapperUnitTests.NestedObject)}'");
    }

    [Fact]
    public void BuildMap_WhenSourceIsDictionary_ThenCanMap()
    {
        //arrange
        var destination = new DataMapperUnitTests.NestedObject();
        Dictionary<string, object> source = new() { { "Prop2", 1 } };

        //act
        var compiledMap = BuildMap(source, destination, [("Prop2", "Prop2")]);

        //assert
        compiledMap.Should().HavePaths([("Prop2", "Prop2")]);
    }

    [Fact]
    public void BuildMap_WhenDestinationIsDictionary_ThenCanMap()
    {
        //arrange
        Dictionary<string, object> destination = new() { { "Prop2", 1 } };
        var source = new DataMapperUnitTests.NestedObject();

        //act
        var compiledMap = BuildMap(source, destination, [("Prop2", "Prop2")]);

        //assert
        compiledMap.Should().HavePaths([("Prop2", "Prop2")]);
    }

    [Fact]
    public void BuildMap_WhenSourceIsJsonNode_ThenCanMap()
    {
        //arrange
        var source = JsonNode.Parse(JsonSerializer.Serialize(new { Prop2 = 1 }));
        var destination = new DataMapperUnitTests.NestedObject();

        //act
        var compiledMap = BuildMap(source, destination, [("Prop2", "Prop2")]);

        //assert
        compiledMap.Should().HavePaths([("Prop2", "Prop2")]);
    }

    [Fact]
    public void BuildMap_WhenMapsILists_ThenCanMap()
    {
        //arrange
        string[] source = ["a"];
        List<string> destination = [];

        //act
        var compiledMap = BuildMap(source, destination, [("0", "0")]);

        //assert
        compiledMap.Should().HavePaths([("0", "0")]);
    }

    [Fact]
    public void BuildMap_WhenMapFromEnumerable_ThenCanMap()
    {
        //arrange
        var source = Enumerable.Range(0, 10);
        List<string> destination = [];

        //act
        var compiledMap = BuildMap(source, destination, [("0", "0")]);

        //assert
        compiledMap.Should().HavePaths([("0", "0")]);
    }

    [Fact]
    public void BuildMap_WhenDestinationIsJsonNode_ThenCanMap()
    {
        //arrange
        var destination = new DataMapperUnitTests.NestedObject();
        var source = JsonNode.Parse(JsonSerializer.Serialize(new { Prop2 = 1 }));

        //act
        var compiledMap = BuildMap(source, destination, [("Prop2", "Prop2")]);

        //assert
        compiledMap.Should().HavePaths([("Prop2", "Prop2")]);
    }

    [Fact]
    public void BuildMap_WhenSourceIsJsonDocument_ThenCanMap()
    {
        //arrange
        var source = JsonDocument.Parse(JsonSerializer.Serialize(new { Prop2 = 1 }));
        var destination = new DataMapperUnitTests.NestedObject();

        //act
        var compiledMap = BuildMap(source, destination, [("Prop2", "Prop2")]);

        //assert
        compiledMap.Should().HavePaths([("Prop2", "Prop2")]);
    }
}