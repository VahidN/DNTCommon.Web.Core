namespace DNTCommon.Web.Core;

public static class EnumCache<T>
    where T : struct, Enum
{
    public static readonly T[] DefinedValues = Enum.GetValues<T>();

    public static readonly string[] DefinedNames = Enum.GetNames<T>();
}
