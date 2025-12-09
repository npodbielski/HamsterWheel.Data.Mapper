using FluentAssertions;
using FluentAssertions.Collections;
using FluentAssertions.Execution;
using HamsterWheel.Data.Mapper.Maps;

namespace HamsterWheel.Data.Mapper.UnitTests;

using ReadOnlyMap = IEnumerable<PropertyMapping>;

public class ReadOnlyMapAssertions(ReadOnlyMap instance)
    : GenericCollectionAssertions<ReadOnlyMap, PropertyMapping, ReadOnlyMapAssertions>(instance, AssertionChain.GetOrCreate())
{
    private readonly ReadOnlyMap _instance = instance;
    protected override string Identifier => "readOnlyMap";

    public AndConstraint<ReadOnlyMapAssertions> HavePaths(Map expected,
        string because = "",
        params object[] becauseArgs)
    {
        Subject.Count().Should().Be(expected.Count, " of expected map that have count '{0}'", _instance.Count());

        for (var i = 0; i < expected.Count; i++)
        {
            var expectedMapping = expected[i];
            var actualMapping = _instance.ElementAt(i);
            string.Join(".", actualMapping.Source.Path.Chunks.Select(c => c.Name)).Should()
                .Be(expectedMapping.Source);
            string.Join(".", actualMapping.Destination.Path.Chunks.Select(c => c.Name)).Should()
                .Be(expectedMapping.Destination);
        }

        return new AndConstraint<ReadOnlyMapAssertions>(this);
    }
}

public static class FluentValidationExtensions
{
    public static ReadOnlyMapAssertions Should(this ReadOnlyMap instance) => new(instance);
}