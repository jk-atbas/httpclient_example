using Spectre.Console;
using Spectre.Console.Json;

namespace Cli.Infrastructure.Commands.Helpers;

/// <summary>
/// Helpers for spectre cli commands
/// </summary>
internal static class CommandHelpers
{
	/// <summary>
	/// Executes the actual send method an gives console output
	/// </summary>
	/// <param name="send">Send delegate</param>
	/// <param name="currentIndex">Current index</param>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>The http response</returns>
	public static async Task<HttpResponseMessage> ExecuteSend(
		Func<CancellationToken, Task<HttpResponseMessage>> send,
		int currentIndex,
		CancellationToken cancellationToken)
	{
		AnsiConsole.MarkupLine($"Current attempt [green]{currentIndex}[/] started!");

		var result = await send.Invoke(cancellationToken);

		AnsiConsole.WriteLine(await result.Content.ReadAsStringAsync(cancellationToken));

		AnsiConsole.Write(new JsonText(await result.Content.ReadAsStringAsync(cancellationToken)));
		AnsiConsole.WriteLine();

		return result;
	}
}
