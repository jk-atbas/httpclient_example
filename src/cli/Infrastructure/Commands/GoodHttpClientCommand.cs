using Cli.Infrastructure.Commands.Helpers;
using Cli.Infrastructure.Commands.Settings;
using Cli.Infrastructure.Services.GoodHttpClient;
using Microsoft.Extensions.Hosting;
using Spectre.Console;
using Spectre.Console.Cli;

namespace Cli.Infrastructure.Commands;

/// <summary>
/// Good service request
/// </summary>
/// <param name="service">The service</param>
/// <param name="lifetime">Cancellation tokens</param>
internal sealed class GoodHttpClientCommand(IGoodHttpClientService service, IHostApplicationLifetime lifetime)
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
				httpResponseMessages.Add(await CommandHelpers.ExecuteSend(service.Send, i, token));
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
}
