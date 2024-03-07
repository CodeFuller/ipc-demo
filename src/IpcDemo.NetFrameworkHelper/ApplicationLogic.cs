using System;
using System.Threading;
using System.Threading.Tasks;
using IpcDemo.NetFrameworkHelper.Interfaces;

namespace IpcDemo.NetFrameworkHelper
{
	internal class ApplicationLogic
	{
		private readonly IIpcServer ipcServer;

		public ApplicationLogic(IIpcServer ipcServer)
		{
			this.ipcServer = ipcServer ?? throw new ArgumentNullException(nameof(ipcServer));
		}

		public async Task Run(CancellationToken cancellationToken)
		{
			await ipcServer.Start(cancellationToken);
		}
	}
}
