namespace DNTCommon.Web.Core;

public static class GuidExtensions
{
    extension(Guid guid)
    {
        public bool IsEmpty => guid == Guid.Empty;
    }

    extension(Guid? guid)
    {
        public bool IsEmpty => guid.HasValue && guid.Value == Guid.Empty;

        public bool IsNullOrEmpty => !guid.HasValue || guid.Value == Guid.Empty;

        public bool IsNotNullOrEmpty => guid.HasValue && guid.Value != Guid.Empty;
    }
}
