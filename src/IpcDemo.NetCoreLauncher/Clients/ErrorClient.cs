using System;
using System.Threading;
using System.Threading.Tasks;
using IpcDemo.Common.Extensions;
using IpcDemo.Common.Interfaces;
using static IpcDemo.Common.ControllerNames;
using static IpcDemo.Common.ErrorControllerActions;

namespace IpcDemo.NetCoreLauncher.Clients
{
	internal class ErrorClient : IErrorClient
	{
		private readonly IIpcClient ipcClient;

		public ErrorClient(IIpcClient ipcClient)
		{
			this.ipcClient = ipcClient ?? throw new ArgumentNullException(nameof(ipcClient));
		}

		public Task TriggerError(CancellationToken cancellationToken)
		{
			return ipcClient.CallServer(ErrorControllerName, ProduceErrorActionName, cancellationToken);
		}
	}
}
