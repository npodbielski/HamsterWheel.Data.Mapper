using System.Diagnostics;

namespace HamsterWheel.Data.Mapper.UnitTests;

public class Benchmark
{
    [Fact(Skip = "Benchmark")]
    public void WhenWithoutMap()
    {
        //arrange
        var destination = new DataMapperUnitTests.NestedObject
        {
            Prop2 = 0,
            Prop3 = 0
        };
        var source = new DataMapperUnitTests.NestedObject
        {
            Prop2 = 10,
            Prop3 = 11
        };

        //act
        var timer = new Stopwatch();
        timer.Start();
        for (var i = 0; i < 1000_000; i++)
        {
            DataMapper.Map(source, destination);
        }

        timer.Stop();
        var t = timer.ElapsedMilliseconds;

        var map = DataMapper.BuildMap(source, destination);
        timer.Reset();
        timer.Start();
        for (var i = 0; i < 1000_000; i++)
        {
            DataMapper.Map(source, destination, cachedMap: map);
        }

        timer.Stop();
        var t2 = timer.ElapsedMilliseconds;
        timer.Reset();
        timer.Start();
        for (var i = 0; i < 1000_000; i++)
        {
            destination.Prop2 = source.Prop2;
            destination.Prop3 = source.Prop3;
        }

        timer.Stop();
        var t3 = timer.ElapsedMilliseconds;
    }
}