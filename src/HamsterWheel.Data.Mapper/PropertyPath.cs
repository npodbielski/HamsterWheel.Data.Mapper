using System.Diagnostics;

namespace HamsterWheel.Data.Mapper;

[DebuggerDisplay("{string.Join(\".\", System.Linq.Enumerable.Select(Chunks, c => c.Name))}")]
public record PropertyPath(PropertyPathChunk[] Chunks)
{
    public bool IsSource => Chunks is [{ IsSource: true }];

    public static PropertyPath From(string propertyPath) => propertyPath == DataMapper.EntireSource
        ? new PropertyPath([new PropertyPathChunk(DataMapper.EntireSource)])
        : new PropertyPath(propertyPath.Split('.').Select(c => new PropertyPathChunk(c)).ToArray());

    public static PropertyPath From(PropertyPathChunk chunk) => new([chunk]);
}