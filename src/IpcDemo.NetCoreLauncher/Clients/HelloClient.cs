using System;
using System.Threading;
using System.Threading.Tasks;
using IpcDemo.Common.Contracts;
using IpcDemo.Common.Extensions;
using IpcDemo.Common.Interfaces;
using static IpcDemo.Common.ControllerNames;
using static IpcDemo.Common.HelloControllerActions;

namespace IpcDemo.NetCoreLauncher.Clients
{
	internal class HelloClient : IHelloClient
	{
		private readonly IIpcClient ipcClient;

		public HelloClient(IIpcClient ipcClient)
		{
			this.ipcClient = ipcClient ?? throw new ArgumentNullException(nameof(ipcClient));
		}

		public Task<HelloResponse> SayHello(HelloRequest request, CancellationToken cancellationToken)
		{
			return ipcClient.CallServer<HelloRequest, HelloResponse>(HelloControllerName, SayHelloActionName, request, cancellationToken);
		}

		public Task<HowAreYouResponse> HowAreYou(CancellationToken cancellationToken)
		{
			return ipcClient.CallServer<HowAreYouResponse>(HelloControllerName, HowAreYouActionName, cancellationToken);
		}
	}
}
