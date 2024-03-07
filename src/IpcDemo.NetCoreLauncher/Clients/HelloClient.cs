using System;
using System.Threading;
using System.Threading.Tasks;
using IpcDemo.Common;
using IpcDemo.Common.Contracts;
using IpcDemo.NetCoreLauncher.Interfaces;

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
			return ipcClient.CallServer<HelloRequest, HelloResponse>(RequestTypes.SayHello, request, cancellationToken);
		}
	}
}
