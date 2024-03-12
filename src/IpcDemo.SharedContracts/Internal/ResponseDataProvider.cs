using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using IpcDemo.Common.Interfaces;
using log4net;

namespace IpcDemo.Common.Internal
{
	internal class ResponseDataProvider : IResponseDataProvider
	{
		private static readonly ILog Log = LogManager.GetLogger("ResponseDataProvider");

		private readonly ConcurrentDictionary<Guid, TaskCompletionSource<byte[]>> _callbackCompletionSources = new ConcurrentDictionary<Guid, TaskCompletionSource<byte[]>>();

		public async Task<byte[]> ProvideResponseData(Guid requestId, Func<Task> clientCall)
		{
			var callbackCompletionSource = new TaskCompletionSource<byte[]>();

			if (!_callbackCompletionSources.TryAdd(requestId, callbackCompletionSource))
			{
				throw new InvalidOperationException($"Duplicated request id: {requestId}");
			}

			await clientCall();

			var taskWithTimeout = Task.WhenAny(callbackCompletionSource.Task, Task.Delay(TimeSpan.FromSeconds(60)));
			var completedTask = await taskWithTimeout;

			_callbackCompletionSources.TryRemove(requestId, out _);

			if (completedTask == callbackCompletionSource.Task)
			{
				Log.Debug($"Completed request {requestId}");
				return await callbackCompletionSource.Task;
			}

			Log.Error($"Request {requestId} has timed out");
			throw new InvalidOperationException($"Request {requestId} has timed out");
		}

		public void PostResponseData(Guid requestId, byte[] responseData)
		{
			var callbackCompletionSource = GetCallbackTaskCompletionSource(requestId);

			callbackCompletionSource.SetResult(responseData);
		}

		private TaskCompletionSource<byte[]> GetCallbackTaskCompletionSource(Guid requestId)
		{
			if (_callbackCompletionSources.TryGetValue(requestId, out var callbackCompletionSource))
			{
				return callbackCompletionSource;
			}

			Log.Error($"Request {requestId} is unknown");
			throw new InvalidOperationException($"Request {requestId} is unknown");
		}
	}
}
