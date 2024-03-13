using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using IpcDemo.Common;
using IpcDemo.Common.Contracts;
using IpcDemo.Common.Interfaces;
using IpcDemo.NetCoreLauncher.Clients;
using IpcDemo.NetCoreLauncher.Interfaces;
using log4net;

namespace IpcDemo.NetCoreLauncher
{
	internal class ApplicationLogic
	{
		private static readonly ILog Log = LogManager.GetLogger("ApplicationLogic");

		private readonly IChildProcessManager childProcessManager;

		private readonly IIpcServer ipcServer;

		private readonly IHelloClient helloClient;

		private readonly IErrorClient errorClient;

		public ApplicationLogic(IChildProcessManager childProcessManager, IIpcServer ipcServer, IHelloClient helloClient, IErrorClient errorClient)
		{
			this.childProcessManager = childProcessManager ?? throw new ArgumentNullException(nameof(childProcessManager));
			this.ipcServer = ipcServer ?? throw new ArgumentNullException(nameof(ipcServer));
			this.helloClient = helloClient ?? throw new ArgumentNullException(nameof(helloClient));
			this.errorClient = errorClient ?? throw new ArgumentNullException(nameof(errorClient));
		}

		public async Task Run(CancellationToken cancellationToken)
		{
			var serverThread = new Thread(() => RunIpcServer(cancellationToken));
			serverThread.Start();

			var childProcessManagerThread = new Thread(() => RunChildProcessManager(cancellationToken));
			childProcessManagerThread.Start();

			await TestHelloRequest(cancellationToken);

			await TestErrorRequest(cancellationToken);

			while (!cancellationToken.IsCancellationRequested)
			{
				var response = await helloClient.HowAreYou(cancellationToken);
				Log.Info($"Q: How are you? A: {response.Status}");

				await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken);
			}

			serverThread.Join();
			childProcessManagerThread.Join();
		}

		private async Task TestHelloRequest(CancellationToken cancellationToken)
		{
			var helloRequest = new HelloRequest
			{
				Name = "Client",
			};

			var response = await helloClient.SayHello(helloRequest, cancellationToken);

			Log.Info($"Hello response: '{response.Greeting}'");
		}

		private async Task TestErrorRequest(CancellationToken cancellationToken)
		{
			try
			{
				await errorClient.TriggerError(cancellationToken);
			}
			catch (IpcRequestFailedException e)
			{
				Log.Info("Error request has expectedly failed: ", e);
			}
		}

		private void RunIpcServer(CancellationToken cancellationToken)
		{
			ipcServer.Run(cancellationToken).GetAwaiter().GetResult();
		}

		private void RunChildProcessManager(CancellationToken cancellationToken)
		{
			var helperExecutablePath = GetHelperExecutablePath();

			var helperProcess = new Process();
			helperProcess.StartInfo.FileName = helperExecutablePath;
			helperProcess.StartInfo.WorkingDirectory = Path.GetDirectoryName(helperExecutablePath);
			helperProcess.StartInfo.Arguments = ipcServer.GetAddress();
			helperProcess.StartInfo.UseShellExecute = false;

			childProcessManager.RunChildProcess(helperProcess, cancellationToken).GetAwaiter().GetResult();
		}

		private static string GetHelperExecutablePath()
		{
			var currentExecutablePath = Assembly.GetExecutingAssembly().Location;
			var currentExecutableDirectory = Path.GetDirectoryName(currentExecutablePath);
			var parent1 = GetParentDirectoryPath(currentExecutableDirectory);
			var parent2 = GetParentDirectoryPath(parent1);
			var parent3 = GetParentDirectoryPath(parent2);
			var parent4 = GetParentDirectoryPath(parent3);

			return Path.Combine(parent4, "IpcDemo.NetFrameworkHelper/bin/Debug/net48/IpcDemo.NetFrameworkHelper.exe");
		}

		private static string GetParentDirectoryPath(string path)
		{
			return Directory.GetParent(path).FullName;
		}
	}
}
