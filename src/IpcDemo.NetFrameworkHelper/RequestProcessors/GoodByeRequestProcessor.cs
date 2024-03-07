using System.Threading;
using System.Threading.Tasks;
using IpcDemo.Common.Contracts;
using IpcDemo.NetFrameworkHelper.Interfaces;
using log4net;

namespace IpcDemo.NetFrameworkHelper.RequestProcessors
{
	internal class GoodByeRequestProcessor : IGoodByeRequestProcessor
	{
		private static readonly ILog Log = LogManager.GetLogger("GoodByeRequestProcessor");

		public Task<GoodByeResponse> ProcessRequest(GoodByeRequest request, CancellationToken cancellationToken)
		{
			Log.Info($"Processing Good bye request for '{request.Name}' ...");

			var response = new GoodByeResponse
			{
				Farewell = $"Good bye, {request.Name}",
			};

			return Task.FromResult(response);
		}
	}
}
