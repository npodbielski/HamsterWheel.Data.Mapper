namespace HamsterWheel.Data.Mapper;

public record PropertyPathChunk(string Name)
{
    public bool IsAny => Name == DataMapper.AnyProperty;
    public bool IsSource => Name == DataMapper.EntireSource;
    public bool HaveWildcard => Name.Contains('*');
}