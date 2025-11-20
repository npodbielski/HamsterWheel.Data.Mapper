using FluentAssertions;

namespace HamsterWheel.Data.Mapper.UnitTests;

using static DataMapper;

partial class DataMapperTests
{
    [Fact]
    public void BuildMap_WhenTwoStarsAndMatchingTypes_ThenIncludeAllProperties()
    {
        //arrange
        Map expected = [("Prop2", "Prop2"), ("Prop3", "Prop3")];
        var destination = new NestedObject();
        var source = new NestedObject();

        //act
        var map = BuildMap(source, destination);

        //assert
        map.Should().HavePaths(expected);
    }

    [Fact]
    public void BuildMap_WhenTwoStarsAndMatchingTypesAndNoMapPassed_ThenIncludeAllProperties()
    {
        //arrange
        Map expected = [("Prop2", "Prop2"), ("Prop3", "Prop3")];
        var destination = new NestedObject();
        var source = new NestedObject();

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
        var destination = new FlatHierarchyClass();
        var source = new NestedObject();

        //act
        var map = BuildMap(source, destination, AnyToAnyMap);

        //assert
        map.Should().HavePaths(expected);
    }

    [Fact]
    public void BuildMap_WhenTwoStarsAndDifferentTypesWithoutMatchingProps_ThenReturnsEmpty()
    {
        //arrange
        var destination = new DummyTokenInfo();
        var source = new NestedObject();

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
        var destination = new GuidKeyEntity();
        var source = new StringKeyEntity();

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
        var destination = new DirectRoot<GuidKeyEntity>(new GuidKeyEntity());
        var source = new StringKeyEntity();

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
        var destination = new StringKeyEntity();
        var source = new DirectRoot<GuidKeyEntity>(new GuidKeyEntity());

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
        var destination = new DirectRoot<StringKeyEntity>(new StringKeyEntity());
        var source = new DirectRoot<GuidKeyEntity>(new GuidKeyEntity());

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
        var destination = new DirectRoot<StringKeyEntity>(new StringKeyEntity());
        var source = new DirectRoot<GuidKeyEntity>(new GuidKeyEntity());

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
        var destination = new DirectRoot<StringKeyEntity>(new StringKeyEntity());
        var source = new DirectRoot<GuidKeyEntity>(new GuidKeyEntity());

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
        var source = new DeeplyNestedRoot<GuidKeyEntity>(new GuidKeyEntity());
        var destination = new DeeplyNestedRoot<StringKeyEntity>(new StringKeyEntity());

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
        var source = new DirectRoot<StringKeyEntity>(new StringKeyEntity());
        var destination = new DeeplyNestedRoot<GuidKeyEntity>(new GuidKeyEntity());

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
        var source = new DeeplyNestedRoot<StringKeyEntity>(new StringKeyEntity());
        var destination = new DirectRoot<GuidKeyEntity>(new GuidKeyEntity());

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
        var source = new DeeplyNestedRoot<GuidKeyEntity>(new GuidKeyEntity());
        var destination = new DeeplyNestedRoot<StringKeyEntity>(new StringKeyEntity());

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
        var destination = new DeeplyNestedRoot<StringKeyEntity>(new StringKeyEntity());
        var source = new DirectRoot<GuidKeyEntity>(new GuidKeyEntity());

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
        var source = new DeeplyNestedRoot<GuidKeyEntity>(new GuidKeyEntity());
        var destination = new DirectRoot<StringKeyEntity>(new StringKeyEntity());

        //act
        var map = BuildMap(source, destination, [("DeeplyNested.Nested.*", "Nested")]);

        //assert
        map.Should().HavePaths(expected);
    }

    [Fact]
    public void
        BuildMap_WhenSourceDeeplyNestedPropertyAndDestinationDeeplyNestedWihtStar_ThenReturnsAnySubMatchingProperties()
    {
        //arrange
        Map expected = [("DeeplyNested.Nested.Key", "DeeplyNested.Nested.Key")];
        var source = new DeeplyNestedRoot<GuidKeyEntity>(new GuidKeyEntity());
        var destination = new DeeplyNestedRoot<StringKeyEntity>(new StringKeyEntity());

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
        var destination = new DeeplyNestedRoot<StringKeyEntity>(new StringKeyEntity());
        var source = new DirectRoot<GuidKeyEntity>(new GuidKeyEntity());

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
        var source = new DeeplyNestedRoot<GuidKeyEntity>(new GuidKeyEntity());
        var destination = new DirectRoot<StringKeyEntity>(new StringKeyEntity());

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
        var destination = new NestedObject();
        var source = 1;

        //act
        var map = BuildMap(source, destination, expected);

        //assert
        map.Should().HavePaths(expected);
    }

    public class GuidKeyEntity
    {
        public Guid Key { get; set; }
    }

    public class StringKeyEntity
    {
        public string Key { get; set; } = null!;
    }

    public class DirectRoot<T>(T nestedObject)
    {
        public T Nested { get; set; } = nestedObject;
    }

    public class DeeplyNestedRoot<T>(T nestedObject)
    {
        public DirectRoot<T> DeeplyNested { get; set; } = new(nestedObject);
    }
}