namespace DNTCommon.Web.Core;

public static class EmojiIcons
{
    /// <summary>
    ///     Returns an output like ★★★☆☆ for stars = 3, totalStars = 5
    /// </summary>
    public static string ToStarRatingIcons(this decimal stars, int totalStars = 5)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(stars, totalStars);

        var starsCount = (int)Math.Round(stars);

        return new string(c: '★', starsCount) + new string(c: '☆', totalStars - starsCount);
    }

    public static class TimeScheduling
    {
        public const string AnalogClock = "🕒";
        public const string Calendar = "📅";
        public const string AlarmClock = "⏰";
        public const string HourGlass = "⏳";
    }

    public static class ProjectAndTask
    {
        public const string Project = "📁";
        public const string Task = "📝";
        public const string Checklist = "✅";
        public const string Bullet = "•";
    }

    public static class StatusAndProgress
    {
        public const string Done = "✔️";
        public const string Pending = "🕐";
        public const string Error = "❌";
        public const string Warning = "⚠️";
        public const string Info = "ℹ️";
        public const string InProgress = "🔄";
        public const string Rocket = "🚀";
        public const string Fire = "🔥";
    }

    public static class Actions
    {
        public const string Start = "▶️";
        public const string Stop = "⏹️";
        public const string Pause = "⏸️";
        public const string Refresh = "🔄";
        public const string Save = "💾";
        public const string Settings = "⚙️";
        public const string Tools = "🛠";
        public const string Lock = "🔒";
        public const string Unlock = "🔓";
        public const string Plus = "➕";
    }

    public static class Communication
    {
        public const string Comment = "💬";
        public const string Notification = "🔔";
        public const string Lightbulb = "💡";
        public const string Attachment = "📎";
        public const string Star = "⭐";
        public const string Question = "❓";
        public const string Pencil = "✏️";
        public const string Pen = "🖊";
    }

    public static class Misc
    {
        public const string Brain = "🧠";
        public const string Web = "🌐";
    }
}
