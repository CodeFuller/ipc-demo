using System.Threading;
using System.Threading.Tasks;
using IpcDemo.Common.Contracts;
using IpcDemo.NetFrameworkHelper.Interfaces;
using log4net;

namespace IpcDemo.NetFrameworkHelper.RequestProcessors
{
	internal class HelloRequestProcessor : IHelloRequestProcessor
	{
		private static readonly ILog Log = LogManager.GetLogger("HelloRequestProcessor");

		public Task<HelloResponse> ProcessRequest(HelloRequest request, CancellationToken cancellationToken)
		{
			Log.Info($"Processing Hello request for '{request.Name}' ...");

			var response = new HelloResponse
			{
				Greeting = $"Hello, {request.Name}",
			};

			return Task.FromResult(response);
		}
	}
}
