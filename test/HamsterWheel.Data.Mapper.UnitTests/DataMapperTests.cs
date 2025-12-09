namespace HamsterWheel.Data.Mapper.UnitTests;

public partial class DataMapperTests
{
    public class FlatHierarchyClass
    {
        public string Prop1 { get; set; }
        public int Prop2 { get; set; }
        public int Prop3 { get; set; }
    }

    public class Root
    {
        public FlatHierarchyClass First { get; set; }
    }

    public class NestedHierarchyClass
    {
        public string Prop1 { get; set; }
        public NestedObject Nested { get; set; }
    }

    public class NestedObject
    {
        public int Prop2 { get; set; }
        public int Prop3 { get; set; }
    }

    public class DummyTokenInfo
    {
        public Guid Id { get; set; }
        public string Value { get; init; }
        public string Name { get; set; } = null!;
        public Uri Link { get; set; } = null!;
    }
}