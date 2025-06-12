using Spectre.Console.Cli;
using System.ComponentModel;

namespace Cli.Infrastructure.Commands.Settings;

/// <summary>
/// Default settings template
/// </summary>
internal class DefaultSettings : CommandSettings
{
	/// <summary>
	/// The amount of requests a command has to execute
	/// </summary>
	[Description("Amount of requests to send")]
	[CommandOption("-c|--count")]
	public int Count { get; init; } = 1;
}
