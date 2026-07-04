namespace RS.Logstream.Tests.Integration.Helpers;

public static class PollingHelper
{
	public static async Task<T> WaitUntilAsync<T>(
		Func<Task<T?>> action,
		Func<T?, bool> isReady,
		TimeSpan? timeout = null,
		TimeSpan? interval = null) where T : class
	{
		timeout ??= TimeSpan.FromSeconds(5);
		interval ??= TimeSpan.FromMilliseconds(50);

		using var cts = new CancellationTokenSource(timeout.Value);
		while (!cts.IsCancellationRequested)
		{
			var result = await action();
			if (isReady(result))
				return result!;

			await Task.Delay(interval.Value);
		}

		throw new TimeoutException($"Condição não satisfeita após {timeout.Value.TotalSeconds}s de polling.");
	}
}
