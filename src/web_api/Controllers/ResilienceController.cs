using Microsoft.AspNetCore.Mvc;

namespace Web_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ResilienceController(ILogger<ResilienceController> logger) : ControllerBase
{
	private readonly Random random = new();
	private int failureCount = 0;
	private const int CircuitTriggerCount = 5;

	[HttpGet("jitter")]
	public async Task<IActionResult> Jitter(int minMs = 100, int maxMs = 1000)
	{
		var delay = this.random.Next(minMs, maxMs);
		logger.LogInformation("Jitter: delaying for {Delay}ms", delay);

		await Task.Delay(delay);

		return this.Ok(new { scenario = "jitter", delay });
	}

	[HttpGet("timeout")]
	public async Task<IActionResult> Timeout(int delayMs = 5000)
	{
		logger.LogInformation("Timeout: delaying for {Delay}ms", delayMs);

		await Task.Delay(delayMs);

		return this.Ok(new { scenario = "timeout", delay = delayMs });
	}

	[HttpGet("error")]
	public IActionResult Error(int code = 500)
	{
		logger.LogWarning("Error: returning HTTP {Code}", code);

		return this.StatusCode(code, new { scenario = "http-error", code });
	}

	[HttpGet("exception")]
	public IActionResult Exception()
	{
		logger.LogError("Exception: throwing simulated exception");

		var error = new ArgumentException("Simulated error");

		return this.StatusCode(503, new { scenario = "http-error", error });
	}

	[HttpGet("flaky")]
	public IActionResult Flaky(double failureRate = 0.5)
	{
		var rnd = this.random.NextDouble();

		if (rnd < failureRate)
		{
			logger.LogWarning("Flaky: simulated failure (rnd={Rnd})", rnd);

			return this.StatusCode(500, new { scenario = "flaky-error", rnd });
		}

		return this.Ok(new { scenario = "flaky-success", rnd });
	}

	[HttpGet("circuit")]
	public IActionResult Circuit()
	{
		this.failureCount++;

		if (this.failureCount <= CircuitTriggerCount)
		{
			logger.LogWarning("Circuit: failure #{Count}", this.failureCount);
			return this.StatusCode(500, new { scenario = "circuit-failure", attempt = this.failureCount });
		}

		logger.LogInformation("Circuit: success at attempt #{Count}", this.failureCount);

		return this.Ok(new { scenario = "circuit-success", attempt = this.failureCount });
	}

	[HttpGet("success")]
	public IActionResult Success()
	{
		return this.StatusCode(200, new { scenario = "success", ok = true });
	}

	/// <summary>
	/// Wählt zufällig eines der definierten Szenarien aus und führt es aus.
	/// </summary>
	[HttpGet("random")]
	public async Task<IActionResult> Random()
	{
		// Definiere alle Szenarien als Delegate-Array
		var scenarios = new Func<Task<IActionResult>>[]
		{
			async () => await this.Jitter(),
			async () => await this.Timeout(),
			() => Task.FromResult(this.Error()),
			() => Task.FromResult(this.Exception()),
			() => Task.FromResult(this.Flaky()),
			() => Task.FromResult(this.Circuit()),
			() => Task.FromResult(this.Success()),
		};

		var index = this.random.Next(scenarios.Length);
		logger.LogInformation("Random scenario index: {Index}", index);

		return await scenarios[index].Invoke();
	}
}
