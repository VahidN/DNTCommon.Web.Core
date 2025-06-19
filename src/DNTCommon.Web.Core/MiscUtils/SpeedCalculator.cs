namespace DNTCommon.Web.Core;

/// <summary>
///     Speed Calculator
/// </summary>
public static class SpeedCalculator
{
    private const long Billion = 1_000_000_000L;
    private const long Million = 1_000_000L;

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
        var operationSpeed = Math.Round(speed / 1024, digits: 2);
        stopwatch.Stop();

        return (string.Create(CultureInfo.InvariantCulture, $"{operationSpeed} KB/s"), timeElapsed);
    }

    /// <summary>
    ///     Calculates the operation's speed in KB/s
    /// </summary>
    public static double ElapsedKBs(this Stopwatch watch, long receivedBytes, int digits = 2)
    {
        ArgumentNullException.ThrowIfNull(watch);

        var speed = receivedBytes / watch.Elapsed.TotalSeconds;
        var operationSpeed = Math.Round(speed / 1024, digits);

        return operationSpeed;
    }

    /// <summary>
    ///     Gets the total elapsed time measured by the current instance
    /// </summary>
    public static long ElapsedNanoseconds(this Stopwatch watch)
    {
        ArgumentNullException.ThrowIfNull(watch);

        return (long)((double)watch.ElapsedTicks / Stopwatch.Frequency * Billion);
    }

    /// <summary>
    ///     Gets the total elapsed time measured by the current instance
    /// </summary>
    public static long ElapsedMicroseconds(this Stopwatch watch)
    {
        ArgumentNullException.ThrowIfNull(watch);

        return (long)((double)watch.ElapsedTicks / Stopwatch.Frequency * Million);
    }
}
