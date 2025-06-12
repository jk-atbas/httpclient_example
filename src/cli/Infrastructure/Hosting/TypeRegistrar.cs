using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Spectre.Console.Cli;

namespace Cli.Infrastructure.Hosting;

/// <summary>
/// Registrar for .NET generic host integration
/// </summary>
/// <remarks>
/// see <see href="https://github.com/spectreconsole/spectre.console/discussions/380#discussioncomment-643500"/>
/// </remarks>
/// <param name="builder">Generic host builder</param>
internal sealed class TypeRegistrar(IHostBuilder builder) : ITypeRegistrar
{
	private readonly TypeResolver typeResolver = new(builder.Build());

	/// <inheritdoc/>
	public void Register(Type service, Type implementation)
	{
		_ = builder.ConfigureServices((_, services) =>
			services.AddSingleton(service, implementation));
	}

	/// <inheritdoc/>
	public void RegisterInstance(Type service, object implementation)
	{
		_ = builder.ConfigureServices((_, services) =>
			services.AddSingleton(service, implementation));
	}

	/// <inheritdoc/>
	public void RegisterLazy(Type service, Func<object> factory)
	{
		_ = builder.ConfigureServices((_, services) =>
			services.AddSingleton(service, factory.Invoke()));
	}

	/// <inheritdoc/>
	public ITypeResolver Build()
	{
		return this.typeResolver;
	}

	/// <summary>
	/// Returns the current generic host
	/// </summary>
	/// <returns>Host</returns>
	public IHost GetCurrentHost()
	{
		return this.typeResolver.GetHost();
	}
}
