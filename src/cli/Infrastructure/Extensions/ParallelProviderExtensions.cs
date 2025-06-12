using System.Collections.Concurrent;

namespace Cli.Infrastructure.Extensions;

internal static class ParallelProviderExtensions
{
	public static async Task<TResult?[]> GetParallelResults<TSource, TResult>(
	   this IEnumerable<TSource> source,
	   Func<TSource, CancellationToken, Task<TResult?>> resultsSeed,
	   Action<Exception>? onErrorAction = null,
	   bool throwOnError = false,
	   int concurrencyLimit = 5,
	   CancellationToken cancellationToken = default)
	   where TResult : notnull
	   where TSource : notnull
	{
		CheckConcurrencyLimit(concurrencyLimit);

		var items = source.ToArray();
		var results = new TResult?[items.Length];

		// Collects all exceptions
		var exceptions = new ConcurrentBag<Exception>();

		var options = new ParallelOptions
		{
			MaxDegreeOfParallelism = concurrencyLimit,
			CancellationToken = cancellationToken,
		};

		await Parallel.ForAsync(0, items.Length, options, async (index, ct) =>
		{
			try
			{
				results[index] = await resultsSeed.Invoke(items[index], ct);
			}
			catch (Exception e)
			{
				onErrorAction?.Invoke(e);

				if (throwOnError)
				{
					exceptions.Add(e);
				}
				else
				{
					results[index] = default;
				}
			}
		});

		CheckForThrow(throwOnError, exceptions);

		return results;
	}

	private static void CheckConcurrencyLimit(int concurrencyLimit)
	{
		if (concurrencyLimit <= 0)
		{
			throw new ArgumentException("Concurrency limit must be greater than 0!", nameof(concurrencyLimit));
		}
	}

	private static void CheckForThrow(bool throwOnError, ConcurrentBag<Exception> exceptions)
	{
		if (throwOnError && !exceptions.IsEmpty)
		{
			throw new AggregateException(exceptions);
		}
	}
}
