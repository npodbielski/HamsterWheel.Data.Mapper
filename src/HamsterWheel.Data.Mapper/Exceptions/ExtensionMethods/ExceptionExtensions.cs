namespace HamsterWheel.Data.Mapper;

public static class ExceptionExtensions
{
    public static void MarkAsPlatform(this Exception exception) => exception.Data["IsPlatformException"] = true;
}