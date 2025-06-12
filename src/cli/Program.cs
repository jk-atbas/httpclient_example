using Cli.Infrastructure.Commands;
using Cli.Infrastructure.Extensions;
using Cli.Infrastructure.Services.GoodHttpClient;
using Cli.Infrastructure.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Spectre.Console.Cli;
using TypeRegistrar = Cli.Infrastructure.Hosting.TypeRegistrar;

namespace Cli;

/// <summary>
/// Main entry point
/// </summary>
internal class Program
{
	/// <summary>
	/// Main method
	/// </summary>
	/// <param name="args">Args</param>
	/// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
	public static async Task Main(string[] args)
	{
		var environmentProvider = SystemEnvironmentProvider.Instance;

		string environment = environmentProvider.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production";

		var appSettungsConfigBuilder = new ConfigurationBuilder()
			.SetBasePath(AppContext.BaseDirectory)
			.AddJsonFile("appsettings.json", false, true)
			.AddJsonFile($"appsettings.{environment}.json", true, true)
			.Build();

		var registrar = new TypeRegistrar(CreateHostBuilder(args, appSettungsConfigBuilder));
		var app = new CommandApp(registrar);

		app.Configure(config =>
		{
			_ = config.AddCommand<GoodHttpClientCommand>("good")
				.WithAlias("g")
				.WithDescription("Good example for http resilience")
				.WithExample("good", "-c", "5")
				.WithExample("g", "--count", "5");

			_ = config.AddCommand<ExhaustionCommand>("exhaust")
				.WithAlias("eh")
				.WithDescription("Causes socket exhaustion")
				.WithExample("eh");

			_ = config.AddCommand<BadHttpClientCommand>("bad")
				.WithAlias("b")
				.WithDescription("Bad example for http resilience")
				.WithExample("bad", "-c", "5")
				.WithExample("g", "--count", "5");
		});

		_ = await app.RunAsync(args);
	}

	private static IHostBuilder CreateHostBuilder(string[] args, IConfiguration appsettingsBuilder)
	{
		return Host.CreateDefaultBuilder(args)
			.ConfigureServices(services =>
			{
				_ = services.AddLogging();
				_ = services.Configure<AppSettings>(appsettingsBuilder);

				// Commands
				_ = services.AddTransient<GoodHttpClientCommand>();
				_ = services.AddTransient<ExhaustionCommand>();
				_ = services.AddTransient<BadHttpClientCommand>();

				// Good handled http client service
				_ = services
					.AddHttpClient<IGoodHttpClientService, GoodHttpClientService>()
					.AddResilience("good");
			});
	}
}
