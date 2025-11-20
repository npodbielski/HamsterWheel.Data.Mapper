using FluentAssertions;

namespace HamsterWheel.Data.Mapper.UnitTests;

using static DataMapper;

partial class DataMapperTests
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
    public void Map_WhenTwoStarsTypeNotMatchAndNested_ThenCopyOnlyRootProperties()
    {
        //arrange
        var expected = new
        {
            Prop1 = "1",
            Prop2 = 0,
            Prop3 = 0
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
            (nameof(NestedHierarchyClass.Prop1), nameof(Root.First) + "." + nameof(NestedHierarchyClass.Prop1))
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
            (nameof(NestedHierarchyClass.Prop1), nameof(Root.First) + "." + nameof(NestedHierarchyClass.Prop1)),
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
            (nameof(NestedHierarchyClass.Prop1), nameof(Root.First) + "." + nameof(NestedHierarchyClass.Prop1)),
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
    public void Map_WhenSourceIsEntireObjectMappedToDestinationProperty_ThenSetsThisProperty()
    {
        //arrange
        var expected = new
        {
            Prop2 = 10,
            Prop3 = 0
        };
        var destination = new NestedObject
        {
            Prop2 = 0,
            Prop3 = 0
        };
        const int source = 10;

        //act
        Map(source, destination, [(".", nameof(destination.Prop2))]);

        //assert
        destination.Should().BeEquivalentTo(expected);
    }
}