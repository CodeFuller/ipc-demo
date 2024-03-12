using System;
using System.Threading;
using System.Threading.Tasks;
using IpcDemo.Common.Interfaces;

namespace IpcDemo.NetFrameworkHelper
{
	internal class ApplicationLogic
	{
		private readonly IIpcServer ipcServer;

		public ApplicationLogic(IIpcServer ipcServer)
		{
			this.ipcServer = ipcServer ?? throw new ArgumentNullException(nameof(ipcServer));
		}

		public Task Run(CancellationToken cancellationToken)
		{
			var serverThread = new Thread(() => RunIpcServer(cancellationToken));
			serverThread.Start();

			serverThread.Join();

			return Task.CompletedTask;
		}

		private void RunIpcServer(CancellationToken cancellationToken)
		{
			ipcServer.Run(cancellationToken).GetAwaiter().GetResult();
		}
	}
}
