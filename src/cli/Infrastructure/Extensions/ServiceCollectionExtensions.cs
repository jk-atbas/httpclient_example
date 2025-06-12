using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Logging;
using Polly;
using System.Threading.RateLimiting;

namespace Cli.Infrastructure.Extensions;

/// <summary>
/// Service collection extensions
/// </summary>
internal static class ServiceCollectionExtensions
{
	/// <summary>
	/// Adds a resilience handler
	/// </summary>
	/// <param name="builder">Http client builder</param>
	/// <param name="identifier">Identifier</param>
	/// <returns>Client builder</returns>
	public static IHttpClientBuilder AddResilience(this IHttpClientBuilder builder, string identifier)
	{
		_ = builder.AddResilienceHandler($"pipeline-{identifier}", (handler, context) =>
		{
			var loggerFactory = context.ServiceProvider.GetRequiredService<ILoggerFactory>();
			var logger = loggerFactory.CreateLogger(identifier);

			handler.AddResilienceHandler(
				logger);
		});

		return builder;
	}

	private static void AddResilienceHandler(
		this ResiliencePipelineBuilder<HttpResponseMessage> builder,
		ILogger logger)
	{
		var tokenBucket = new TokenBucketRateLimiter(new TokenBucketRateLimiterOptions()
		{
			AutoReplenishment = true,

			// Enable a unlimited queue to circumvent potential RateLimiterRejectedExceptions
			QueueLimit = int.MaxValue,
			QueueProcessingOrder = QueueProcessingOrder.OldestFirst,

			// For example a max of 10 tokens at the same time
			TokenLimit = 10,

			// For every second replenish e.g. 10 tokens => 10 RPS
			ReplenishmentPeriod = TimeSpan.FromSeconds(1),
			TokensPerPeriod = 10,
		});

		_ = builder.AddRateLimiter(new HttpRateLimiterStrategyOptions
		{
			RateLimiter = async _ =>
			{
				var lease = await tokenBucket.AcquireAsync();

				return lease;
			},
			OnRejected = _ =>
			{
				logger.LogInformation("Request rejected due to rate-limit!");

				return ValueTask.CompletedTask;
			},
		});

		_ = builder.AddConcurrencyLimiter(10, int.MaxValue);

		_ = builder.AddRetry(new HttpRetryStrategyOptions
		{
			BackoffType = DelayBackoffType.Exponential,
			MaxRetryAttempts = 6,
			ShouldHandle = args => HandleFailure(args.Outcome),
			OnRetry = args =>
			{
				logger.LogInformation(
					"Retrying request due to {outcome}. Current retry {retryNumber}",
					args.Outcome,
					args.AttemptNumber + 1);

				return ValueTask.CompletedTask;
			},
			UseJitter = true,
			MaxDelay = TimeSpan.FromSeconds(180),
		});

		_ = builder.AddCircuitBreaker(new HttpCircuitBreakerStrategyOptions
		{
			BreakDuration = TimeSpan.FromSeconds(10),
			FailureRatio = 0.2d,
			SamplingDuration = 6 * TimeSpan.FromSeconds(30),
			ShouldHandle = args => HandleFailure(args.Outcome),
		});

		_ = builder.AddTimeout(new HttpTimeoutStrategyOptions
		{
			Name = "Attempt timeout",
			Timeout = TimeSpan.FromSeconds(30),
			OnTimeout = args =>
			{
				logger.LogInformation("Request timeout of {seconds}s reached!", args.Timeout);

				return ValueTask.CompletedTask;
			},
		});
	}

	private static ValueTask<bool> HandleFailure(Outcome<HttpResponseMessage> outcome)
	{
		return ValueTask.FromResult(outcome switch
		{
			{ Exception: HttpRequestException or HttpIOException or TimeoutException } => true,
			{ Result.IsSuccessStatusCode: false } => true,
			_ => false,
		});
	}
}
