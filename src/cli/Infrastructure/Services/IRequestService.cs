namespace Cli.Infrastructure.Services;

/// <summary>
/// Request service contract
/// </summary>
public interface IRequestService
{
	/// <summary>
	/// Send a request and receive a rep for it
	/// </summary>
	/// <param name="cancellationToken">Cancellation token</param>
	/// <returns>The response message</returns>
	Task<HttpResponseMessage> Send(CancellationToken cancellationToken);
}
