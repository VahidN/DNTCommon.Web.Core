namespace DNTCommon.Web.Core;

/// <summary>
///     Speed Calculator
/// </summary>
public static class SpeedCalculator
{
    /// <summary>
    ///     Calculates the operation's speed in KB/s
    /// </summary>
    /// <param name="callback"></param>
    /// <returns></returns>
    public static async Task<(string OperationSpeed, TimeSpan TimeElapsed)> StartAsync(Func<Task<long>> callback)
    {
        ArgumentNullException.ThrowIfNull(callback);

        Stopwatch stopwatch = new();
        stopwatch.Start();
        var bytes = await callback();
        var timeElapsed = stopwatch.Elapsed;
        var speed = bytes / timeElapsed.TotalSeconds;
        var operationSpeed = Math.Round(speed / 1024, 2);
        stopwatch.Stop();

        return (string.Create(CultureInfo.InvariantCulture, $"{operationSpeed} KB/s"), timeElapsed);
    }
}