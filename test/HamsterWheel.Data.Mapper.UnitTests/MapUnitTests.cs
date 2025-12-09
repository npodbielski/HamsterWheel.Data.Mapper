using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Nodes;
using FluentAssertions;
using HamsterWheel.Data.Mapper.Maps;

namespace HamsterWheel.Data.Mapper.UnitTests;

using static DataMapper;

partial class DataMapperUnitTests
{
    [Fact]
    public void Map_WhenTwoStarsNoNestedAndTypesMatch_ThenCopyAllProperties()
    {
        //arrange
        var expected = new
        {
            Prop2 = 10,
            Prop3 = 11
        };
        var destination = new NestedObject
        {
            Prop2 = 0,
            Prop3 = 0
        };
        var source = new NestedObject
        {
            Prop2 = 10,
            Prop3 = 11
        };

        //act
        Map(source, destination);

        //assert
        destination.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Map_WhenTwoStarsTypeNotMatchAndNotNested_ThenCopyAllProperties()
    {
        //arrange
        var expected = new
        {
            Prop1 = "1",
            Prop2 = 10,
            Prop3 = 11
        };
        var destination = new FlatHierarchyClass
        {
            Prop1 = "1",
            Prop2 = 0,
            Prop3 = 0
        };
        var source = new NestedObject
        {
            Prop2 = 10,
            Prop3 = 11
        };

        //act
        Map(source, destination, [(AnyProperty, AnyProperty)]);

        //assert
        destination.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Map_WhenPartialPropertyNameWithWildCard_ThenCopyAllProperties()
    {
        //arrange
        var expected = new
        {
            Prop1 = "1",
            Prop2 = 10,
            Prop3 = 11
        };
        var destination = new FlatHierarchyClass
        {
            Prop1 = "1",
            Prop2 = 0,
            Prop3 = 0
        };
        var source = new NestedObject
        {
            Prop2 = 10,
            Prop3 = 11
        };

        //act
        Map(source, destination, [("Prop*", "*")]);

        //assert
        destination.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Map_WhenTwoStarsTypeNotMatchAndNested_ThenCopyRootAndNestedProperties()
    {
        //arrange
        var expected = new
        {
            Prop1 = "1",
            Prop2 = 10,
            Prop3 = 11
        };
        var destination = new FlatHierarchyClass
        {
            Prop1 = "_",
            Prop2 = 0,
            Prop3 = 0
        };
        var source = new NestedHierarchyClass
        {
            Prop1 = "1",
            Nested = new NestedObject
            {
                Prop2 = 10,
                Prop3 = 11
            }
        };

        //act
        Map(source, destination);

        //assert
        destination.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Map_WhenComplexMapTypeNotMatchAndNested_ThenCopyAllProperties()
    {
        //arrange
        var expected = new
        {
            Prop1 = "1",
            Prop2 = 10,
            Prop3 = 11
        };
        var destination = new FlatHierarchyClass
        {
            Prop1 = "_",
            Prop2 = 0,
            Prop3 = 0
        };
        var source = new NestedHierarchyClass
        {
            Prop1 = "1",
            Nested = new NestedObject
            {
                Prop2 = 10,
                Prop3 = 11
            }
        };

        //act
        Map(source, destination, [(AnyProperty, AnyProperty), ("Nested", AnyProperty)]);

        //assert
        destination.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Map_WhenTwoConcreteMapsFirstCreatesAndSecondAddProp_ThenMapsCorrectly()
    {
        //arrange
        var expected = new FlatHierarchyClass
        {
            Prop1 = "Test1",
            Prop2 = 23,
            Prop3 = 2
        };
        var obj = new Root();
        var source = new NestedHierarchyClass
        {
            Prop1 = expected.Prop1,
            Nested = new NestedObject
            {
                Prop2 = expected.Prop2,
                Prop3 = expected.Prop3,
            }
        };

        //act
        Map(source, obj, [
            ("Nested", nameof(Root.First)),
            ("Prop1", nameof(Root.First) + ".Prop1")
        ]);

        //assert
        obj.Should().NotBeNull();
        obj.First.Should().NotBeNull();
        obj.First.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Map_WhenTwoMapsAnyFirstThenConcreteSecond_ThenMapsCorrectly()
    {
        //arrange
        var expected = new FlatHierarchyClass
        {
            Prop1 = "Test1",
            Prop2 = 23,
            Prop3 = 2
        };
        var obj = new Root();
        var source = new NestedHierarchyClass
        {
            Prop1 = expected.Prop1,
            Nested = new NestedObject
            {
                Prop2 = expected.Prop2,
                Prop3 = expected.Prop3,
            }
        };

        //act
        Map(source, obj,
        [
            ("Nested.*", nameof(Root.First)),
            (nameof(NestedHierarchyClass.Prop1), $"{nameof(Root.First)}.{nameof(NestedHierarchyClass.Prop1)}")
        ]);

        //assert
        obj.Should().NotBeNull();
        obj.First.Should().NotBeNull();
        obj.First.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void
        Map_WhenTwoMapsConcreteFirstAndSecondLast_ThenMapsAndDoNotOverwritePreviousProps()
    {
        //arrange
        var expected = new FlatHierarchyClass
        {
            Prop1 = "Test1",
            Prop2 = 23,
            Prop3 = 2
        };
        var obj = new Root();
        var source = new NestedHierarchyClass
        {
            Prop1 = expected.Prop1,
            Nested = new NestedObject
            {
                Prop2 = expected.Prop2,
                Prop3 = expected.Prop3,
            }
        };

        //act
        Map(source, obj,
        [
            (nameof(NestedHierarchyClass.Prop1), $"{nameof(Root.First)}.{nameof(NestedHierarchyClass.Prop1)}"),
            ("Nested.*", nameof(Root.First))
        ]);

        //assert
        obj.Should().NotBeNull();
        obj.First.Should().NotBeNull();
        obj.First.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Map_WhenTwoAnyMaps_ThenMapsAndDoNotOverwritePreviousProps()
    {
        //arrange
        var expected = new FlatHierarchyClass
        {
            Prop1 = "Test1",
            Prop2 = 23,
            Prop3 = 2
        };
        var obj = new Root();
        var source = new NestedHierarchyClass
        {
            Prop1 = expected.Prop1,
            Nested = new NestedObject
            {
                Prop2 = expected.Prop2,
                Prop3 = expected.Prop3,
            }
        };

        //act
        Map(source, obj, [
            (nameof(NestedHierarchyClass.Prop1), $"{nameof(Root.First)}.{nameof(NestedHierarchyClass.Prop1)}"),
            ("Nested.*", nameof(Root.First))
        ]);

        //assert
        obj.Should().NotBeNull();
        obj.First.Should().NotBeNull();
        obj.First.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Map_WhenPropertyIsNullAndTargetTypeIsValueType_ThenSetsTargetToDefault()
    {
        //arrange
        var expected = new
        {
            Prop2 = 0
        };
        var destination = new NestedObject
        {
            Prop2 = 10,
            Prop3 = 323230
        };
        var source = new
        {
            Prop2 = (string?)null,
            Prop3 = 11
        };

        //act
        Map(source, destination, [(nameof(source.Prop2), nameof(source.Prop2))]);

        //assert
        destination.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Map_WhenSourcePropDoesNotHaveGetter_ThenThrowsException()
    {
        //arrange
        var destination = new ReadOnlyPropertyClass();
        var source = new { Test = 1 };
        var action = () =>
            Map(source, destination, [(nameof(source.Test), nameof(ReadOnlyPropertyClass.Test))]);

        //act && assert
        action.Should().Throw<MissingPropertySetterException>()
            .WithMessage($"Property: '{nameof(ReadOnlyPropertyClass.Test)}' does not have a setter");
    }

    [Fact]
    public void Map_WhenDestinationPropDoesNotHaveSetter_ThenThrowsException()
    {
        //arrange
        var source = new ReadOnlyPropertyClass();
        var destination = new { Test = 1 };
        var action = () =>
            Map(source, destination, [(nameof(source.Test), nameof(ReadOnlyPropertyClass.Test))]);

        //act && assert
        action.Should().Throw<MissingPropertySetterException>()
            .WithMessage($"Property: '{nameof(WriteOnlyPropertyClass.Test)}' does not have a setter");
    }

    [Fact]
    public void Map_WhenMapsEntireSourceToProperty_ThenSetsDestinationProperty()
    {
        //arrange
        var source = 10;
        var destination = new NestedObject();
        var expected = new { Prop2 = 10 };

        //act
        Map(source, destination, [(EntireSource, nameof(destination.Prop2))]);

        //assert
        destination.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Map_WhenCustomMapProvidedWithInvalidSourcePropGetter_ThenThrowsException()
    {
        //arrange
        ReadOnlyCollection<PropertyMapping> customMap =
            new([
                new(
                    new PropertyAccessor(typeof(object), typeof(object),
                        new PropertyPath([new PropertyPathChunk("foo")]), null, (_, _) => { }),
                    new PropertyAccessor(typeof(object), typeof(object),
                        new PropertyPath([new PropertyPathChunk("foo")]), o => o, (_, _) => { }))
            ]);
        var action = () => Map(new { }, new { }, cachedMap: customMap);

        //act && assert
        var exception = action.Should().Throw<MissingPropertyGetterException>();

        //assert
        exception.WithMessage("Property: 'foo' does not have a getter");
    }

    [Fact]
    public void Map_WhenCustomMapProvidedWithInvalidDestinationPropSetter_ThenThrowsException()
    {
        //arrange
        ReadOnlyCollection<PropertyMapping> customMap =
            new([
                new(
                    new PropertyAccessor(typeof(object), typeof(object),
                        new PropertyPath([new PropertyPathChunk("foo")]), o => o, (_, _) => { }),
                    new PropertyAccessor(typeof(object), typeof(object),
                        new PropertyPath([new PropertyPathChunk("foo")]), o => o, null))
            ]);
        var action = () => Map(new { }, new { }, cachedMap: customMap);

        //act && assert
        var exception = action.Should().Throw<MissingPropertySetterException>();

        //assert
        exception.WithMessage("Property: 'foo' does not have a setter");
    }

    [Fact]
    public void Map_WhenSourceIsDictionary_ThenCanMap()
    {
        //arrange
        var destination = new NestedObject();
        Dictionary<string, object> source = new() { { "Prop2", 1 } };

        //act
        Map(source, destination, [("Prop2", "Prop2")]);

        //assert
        destination.Should().BeEquivalentTo(new { Prop2 = 1 });
    }

    [Fact]
    public void Map_WhenDestinationIsDictionary_ThenCanMap()
    {
        //arrange
        var source = new NestedObject()
        {
            Prop2 = 1
        };
        Dictionary<string, object> destination = new() { { "Prop2", 0 } };

        //act
        Map(source, destination, [("Prop2", "Prop2")]);

        //assert
        destination.Should().BeEquivalentTo(new Dictionary<string, object> { { "Prop2", 1 } });
    }

    [Fact]
    public void Map_WhenSourceIsJsonNode_ThenCanMap()
    {
        //arrange
        var source = JsonNode.Parse(JsonSerializer.Serialize(new { Prop2 = 1 }));
        var destination = new NestedObject();

        //act
        Map(source, destination, [("Prop2", "Prop2")]);

        //assert
        destination.Should().BeEquivalentTo(new { Prop2 = 1 });
    }

    [Fact]
    public void Map_WhenDestinationIsJsonNode_ThenCanMap()
    {
        //arrange
        var destination = new NestedObject();
        var source = JsonNode.Parse(JsonSerializer.Serialize(new { Prop2 = 1 }));

        //act
        Map(source, destination, [("Prop2", "Prop2")]);

        //assert
        destination.Should().BeEquivalentTo(new { Prop2 = 1 });
    }

    [Fact]
    public void Map_WhenMapsILists_ThenCanMap()
    {
        //arrange
        List<string> source = ["a"];
        List<string> destination = [];

        //act
        Map(source, destination, [("0", "0")]);

        //assert
        destination.Should().BeEquivalentTo(["a"]);
    }

    [Fact]
    public void Map_WhenMapFromEnumerable_ThenCanMap()
    {
        //arrange
        var source = Enumerable.Range(0, 10);
        List<string> destination = [];

        //act
        Map(source, destination, [("0", "0")]);

        //assert
        destination.Should().BeEquivalentTo(["0"]);
    }

    [Fact]
    public void Map_WhenSourceIsJsonDocument_ThenCanMap()
    {
        //arrange
        var source = JsonDocument.Parse(JsonSerializer.Serialize(new { Prop2 = 1 }));
        var destination = new NestedObject();

        //act
        Map(source, destination, [("Prop2", "Prop2")]);

        //assert
        destination.Should().BeEquivalentTo(new { Prop2 = 1 });
    }
}