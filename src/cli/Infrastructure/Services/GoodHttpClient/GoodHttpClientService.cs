using Cli.Infrastructure.Settings;
using Microsoft.Extensions.Options;

namespace Cli.Infrastructure.Services.GoodHttpClient;
/// <summary>
/// Represents a good http endpoint request
/// </summary>
/// <param name="httpClient">Http client</param>
/// <param name="settings">App settings</param>
internal class GoodHttpClientService(HttpClient httpClient, IOptionsMonitor<AppSettings> settings)
	: IGoodHttpClientService
{
	/// <inheritdoc/>
	public async Task<HttpResponseMessage> Send(CancellationToken cancellationToken)
	{
		var baseUri = new Uri(settings.CurrentValue.ApiUrl);

		return await httpClient.GetAsync(baseUri, cancellationToken);
	}
}
