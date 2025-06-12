using Cli.Infrastructure.Commands.Helpers;
using Cli.Infrastructure.Commands.Settings;
using Cli.Infrastructure.Settings;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Cli.Infrastructure.Commands;

/// <summary>
/// Requests without resilience
/// </summary>
/// <param name="client">Http client</param>
/// <param name="appsettings">App settings</param>
/// <param name="lifetime">Host lifetime</param>
internal class BadHttpClientCommand(
	HttpClient client,
	IOptionsMonitor<AppSettings> appsettings,
	IHostApplicationLifetime lifetime)
	: AsyncCommand<DefaultSettings>
{
	/// <inheritdoc/>
	public override async Task<int> ExecuteAsync(CommandContext context, DefaultSettings settings)
	{
		var token = lifetime.ApplicationStopping;
		List<HttpResponseMessage> httpResponseMessages = [];

		try
		{
			for (int i = 0; i < settings.Count; i++)
			{
				httpResponseMessages.Add(await CommandHelpers.ExecuteSend(this.GetResult, i, token));
			}

			AnsiConsole
				.MarkupLine($"[red]{httpResponseMessages.Count(m => !m.IsSuccessStatusCode)}[/]/[green]{httpResponseMessages.Count}[/] failed!");

			return 0;
		}
		catch (Exception e)
		{
			AnsiConsole.WriteLine(e.Message);

			return 1;
		}
	}

	private async Task<HttpResponseMessage> GetResult(CancellationToken cancellationToken)
	{
		return await client.GetAsync(appsettings.CurrentValue.ApiUrl, cancellationToken);
	}
}
