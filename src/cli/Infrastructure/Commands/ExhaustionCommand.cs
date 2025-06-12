using Cli.Infrastructure.Extensions;
using Cli.Infrastructure.Settings;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Json;

namespace Cli.Infrastructure.Commands;

/// <summary>
/// Command thats supposed to cause an socket exception
/// </summary>
/// <param name="settings">Appsettings</param>
/// <param name="lifetime">Host lifetime</param>
internal class ExhaustionCommand(IOptionsMonitor<AppSettings> settings, IHostApplicationLifetime lifetime)
	: AsyncCommand
{
	/// <inheritdoc/>
	public override async Task<int> ExecuteAsync(CommandContext context)
	{
		using var httpClient = new HttpClient();

		var result = (await Enumerable.Range(0, 10000)
			.GetParallelResults(
				this.SendRequest,
				this.OnError,
				concurrencyLimit: 1000,
				cancellationToken: lifetime.ApplicationStopping))
			.OfType<HttpResponseMessage>();

		AnsiConsole
			.MarkupLine($"[red]{result.Count(m => !m.IsSuccessStatusCode)}[/]/[green]{result.Count()}[/] failed!");

		return 0;
	}

	private async Task<HttpResponseMessage?> SendRequest(int index, CancellationToken cancellationToken)
	{
		using var httpClient = new HttpClient();

		AnsiConsole.MarkupLine($"Current attempt [green]{index}[/] started!");

		var result = await httpClient.GetAsync(settings.CurrentValue.ApiUrl, cancellationToken);

		AnsiConsole.Write(new JsonText(await result.Content.ReadAsStringAsync(cancellationToken)));
		AnsiConsole.WriteLine();

		return result;
	}

	private void OnError(Exception error)
	{
		AnsiConsole.WriteException(error);
	}
}
