// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace HamsterWheel.Data.Mapper.UnitTests;

public partial class DataMapperUnitTests
{
    public class FlatHierarchyClass
    {
        public string Prop1 { get; set; } = null!;
        public int Prop2 { get; set; }
        public int Prop3 { get; set; }
    }

    public class Root
    {
        public FlatHierarchyClass First { get; set; } = null!;
    }

    public class ReadOnlyRoot
    {
        public FlatHierarchyClass First { get; } = new();
    }

    public class NestedHierarchyClass
    {
        public string Prop1 { get; set; } = null!;
        public NestedObject Nested { get; set; } = null!;
    }

    public class NestedObject
    {
        public int Prop2 { get; set; }
        public int Prop3 { get; set; }
    }

    public class DummyTokenInfo
    {
        public Guid Id { get; set; }
        public string Value { get; init; } = null!;
        public string Name { get; set; } = null!;
        public Uri Link { get; set; } = null!;
    }

    public class ReadOnlyPropertyClass
    {
        public string Test { get; } = "1";
    }

    public class WriteOnlyPropertyClass
    {
        // ReSharper disable once NotAccessedField.Local // used for test
        private string _test = null!;

        public string Test
        {
            set => _test = value;
        }
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