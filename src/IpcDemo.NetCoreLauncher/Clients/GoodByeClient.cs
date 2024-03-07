using System;
using System.Threading;
using System.Threading.Tasks;
using IpcDemo.Common;
using IpcDemo.Common.Contracts;
using IpcDemo.NetCoreLauncher.Interfaces;

namespace IpcDemo.NetCoreLauncher.Clients
{
	internal class GoodByeClient : IGoodByeClient
	{
		private readonly IIpcClient ipcClient;

		public GoodByeClient(IIpcClient ipcClient)
		{
			this.ipcClient = ipcClient ?? throw new ArgumentNullException(nameof(ipcClient));
		}

		public Task<GoodByeResponse> SayGoodBye(GoodByeRequest request, CancellationToken cancellationToken)
		{
			return ipcClient.CallServer<GoodByeRequest, GoodByeResponse>(RequestTypes.SayGoodBye, request, cancellationToken);
		}
	}
}
