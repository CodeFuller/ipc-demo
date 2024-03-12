using IpcDemo.Common.Contracts;
using System.Threading.Tasks;
using System.Threading;
using IpcDemo.Common.Interfaces;
using log4net;
using static IpcDemo.Common.ControllerNames;
using static IpcDemo.Common.HelloCallbackControllerActions;

namespace IpcDemo.NetCoreLauncher.Controllers
{
	internal class HelloCallbackController : IIpcController
	{
		private static readonly ILog Log = LogManager.GetLogger("HelloCallbackController");

		public void RegisterActions(IIpcServer ipcServer)
		{
			ipcServer.RegisterAction<HelloCallbackRequest, HelloCallbackResponse>(HelloCallbackControllerName, HelloCallbackActionName, HelloCallback);
		}

		public Task<HelloCallbackResponse> HelloCallback(HelloCallbackRequest request, CancellationToken cancellationToken)
		{
			Log.Info($"Hello callback: '{request.Greeting}'");

			var response = new HelloCallbackResponse
			{
				Name = "Server",
			};

			return Task.FromResult(response);
		}
	}
}
