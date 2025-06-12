using Cli.Infrastructure.Commands.Settings;
using Cli.Infrastructure.Services.GoodHttpClient;
using Microsoft.Extensions.Hosting;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Json;

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
				AnsiConsole.MarkupLine($"Current attempt [green]{i}[/] started!");

				var result = await service.Send(token);
				httpResponseMessages.Add(result);

				AnsiConsole.WriteLine(await result.Content.ReadAsStringAsync(token));

				AnsiConsole.Write(new JsonText(await result.Content.ReadAsStringAsync(token)));
				AnsiConsole.WriteLine();
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
