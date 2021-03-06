using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Angelo.Jobs.Server
{
	public class InfiniteRetryProcessor : IProcessor
	{
		private IProcessor _inner;
		private ILogger _logger;

		public InfiniteRetryProcessor(
			IProcessor inner,
			ILoggerFactory loggerFactory)
		{
			_inner = inner;
			_logger = loggerFactory.CreateLogger<InfiniteRetryProcessor>();
		}

		public override string ToString() => _inner.ToString();

		public async Task ProcessAsync(ProcessingContext context)
		{
			while (!context.IsStopping)
			{
				try
				{
					await _inner.ProcessAsync(context);
					return;
				}
				catch (OperationCanceledException)
				{
					return;
				}
				catch (Exception ex)
				{
					_logger.LogWarning($"Prcessor '{_inner.ToString()}' failed: '{ex.Message}'. Retrying...");
				}
			}
		}
	}
}
