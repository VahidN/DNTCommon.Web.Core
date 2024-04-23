namespace DNTCommon.Web.Core;

/// <summary>
///     Provides info about the web server's application
/// </summary>
public class ApplicationProcess
{
	/// <summary>
	///     Returns a string array containing the command-line arguments for the current process.
	/// </summary>
	public string ProcessArguments { set; get; } = default!;

	/// <summary>
	///     Returns the path of the executable that started the currently executing process.
	/// </summary>
	public string ProcessPath { set; get; } = default!;

	/// <summary>
	///     Gets the name of the process module.
	/// </summary>
	public string ProcessName { set; get; } = default!;

	/// <summary>
	///     Gets the unique identifier for the associated process.
	/// </summary>
	public string ProcessId { set; get; } = default!;

	/// <summary>
	///     Gets the time that the associated process was started.
	/// </summary>
	public DateTime ProcessStartTime { set; get; }

	/// <summary>
	///     Gets the process architecture of the currently running app.
	/// </summary>
	public string ProcessArchitecture { set; get; } = default!;
}