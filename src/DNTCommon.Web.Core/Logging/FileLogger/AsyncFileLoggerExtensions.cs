using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DNTCommon.Web.Core;

public static class AsyncFileLoggerExtensions
{
    /// <summary>
    ///     ثبت لاگر و بارگذاری خودکار تنظیمات از بخش FileLogging در IConfiguration
    /// </summary>
    public static ILoggingBuilder AddAsyncFileLogger(this ILoggingBuilder builder, FileLoggerOptions fileLoggerOptions)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(fileLoggerOptions);

        builder.Services.AddSingleton(Options.Create(fileLoggerOptions));
        builder.Services.AddSingleton<ILoggerProvider, AsyncFileLoggerProvider>();

        return builder;
    }
}
