using Microsoft.Extensions.Hosting;
using Spectre.Console.Cli;

namespace Cli.Infrastructure.Hosting;

/// <summary>
/// Integration for a .NET generic host
/// </summary>
/// <remarks>
/// <see href="https://github.com/spectreconsole/spectre.console/discussions/380#discussioncomment-643500"/>
/// </remarks>
/// <param name="host">Generic host</param>
internal class TypeResolver(IHost host) : ITypeResolver
{
	/// <inheritdoc/>
	public object? Resolve(Type? type)
	{
		ArgumentNullException.ThrowIfNull(type);

		return host.Services.GetService(type);
	}

	/// <summary>
	/// Return current host
	/// </summary>
	/// <returns>Host</returns>
	public IHost GetHost()
	{
		return host;
	}
}
