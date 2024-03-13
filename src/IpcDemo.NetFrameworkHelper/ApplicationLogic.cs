using System;
using System.Threading;
using System.Threading.Tasks;
using IpcDemo.Common.Interfaces;
using IpcDemo.NetFrameworkHelper.Internal;
using log4net;

namespace IpcDemo.NetFrameworkHelper
{
	internal class ApplicationLogic
	{
		private static readonly ILog Log = LogManager.GetLogger("ApplicationLogic");

		private readonly IIpcServer ipcServer;

		public ApplicationLogic(IIpcServer ipcServer)
		{
			this.ipcServer = ipcServer ?? throw new ArgumentNullException(nameof(ipcServer));
		}

		public Task Run(CancellationToken cancellationToken)
		{
			var serverThread = new Thread(() => RunIpcServer(cancellationToken));
			serverThread.Start();

			var checkParentProcessStatusThread = new Thread(CheckParentProcessStatus);
			checkParentProcessStatusThread.Start();

			serverThread.Join();
			checkParentProcessStatusThread.Join();

			return Task.CompletedTask;
		}

		private void RunIpcServer(CancellationToken cancellationToken)
		{
			ipcServer.Run(cancellationToken).GetAwaiter().GetResult();
		}

		private void CheckParentProcessStatus()
		{
			var parentProcess = ParentProcessUtilities.GetParentProcess();

			Log.Info($"Parent process id: {parentProcess.Id}");

			parentProcess.WaitForExit();

			Log.Info("Parent process has exited. Exiting from child process ...");
			Environment.Exit(0);
		}
	}
}
