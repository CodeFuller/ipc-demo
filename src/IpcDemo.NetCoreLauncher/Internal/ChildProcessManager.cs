using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using IpcDemo.NetCoreLauncher.Interfaces;
using log4net;

namespace IpcDemo.NetCoreLauncher.Internal
{
	internal class ChildProcessManager : IChildProcessManager
	{
		private static readonly ILog Log = LogManager.GetLogger("ChildProcessManager");

		public async Task RunChildProcess(Process process, CancellationToken cancellationToken)
		{
			while (!cancellationToken.IsCancellationRequested)
			{
				process.Start();
				Log.Info($"Child process has started: {process.StartInfo.FileName} ...");

				await process.WaitForExitAsync(cancellationToken);
				Log.Info($"Child process has stopped: {process.StartInfo.FileName}");
			}
		}
	}
}
