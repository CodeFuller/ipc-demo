using System;
using System.Threading;
using System.Threading.Tasks;
using IpcDemo.Common.Contracts;
using IpcDemo.Common.Interfaces;
using static IpcDemo.Common.ControllerNames;
using static IpcDemo.Common.HelloCallbackControllerActions;

namespace IpcDemo.NetFrameworkHelper.Clients
{
	internal class HelloCallbackClient : IHelloCallbackClient
	{
		private readonly IIpcClient ipcClient;

		public HelloCallbackClient(IIpcClient ipcClient)
		{
			this.ipcClient = ipcClient ?? throw new ArgumentNullException(nameof(ipcClient));
		}

		public Task<HelloCallbackResponse> Callback(HelloCallbackRequest request, CancellationToken cancellationToken)
		{
			return ipcClient.CallServer<HelloCallbackRequest, HelloCallbackResponse>(HelloCallbackControllerName, HelloCallbackActionName, request, cancellationToken);
		}
	}
}
