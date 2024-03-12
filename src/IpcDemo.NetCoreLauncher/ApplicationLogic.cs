using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using IpcDemo.Common.Contracts;
using IpcDemo.Common.Interfaces;
using IpcDemo.NetCoreLauncher.Clients;
using log4net;

namespace IpcDemo.NetCoreLauncher
{
	internal class ApplicationLogic
	{
		private static readonly ILog Log = LogManager.GetLogger("ApplicationLogic");

		private readonly IIpcServer ipcServer;

		private readonly IHelloClient helloClient;

		public ApplicationLogic(IIpcServer ipcServer, IHelloClient helloClient)
		{
			this.ipcServer = ipcServer ?? throw new ArgumentNullException(nameof(ipcServer));
			this.helloClient = helloClient ?? throw new ArgumentNullException(nameof(helloClient));
		}

		public async Task Run(CancellationToken cancellationToken)
		{
			var serverThread = new Thread(() => RunIpcServer(cancellationToken));
			serverThread.Start();

			Log.Info("Starting helper process ...");

			var helperExecutablePath = GetHelperExecutablePath();

			var helperProcess = new Process();
			helperProcess.StartInfo.FileName = helperExecutablePath;
			helperProcess.StartInfo.WorkingDirectory = Path.GetDirectoryName(helperExecutablePath);
			helperProcess.StartInfo.Arguments = ipcServer.GetAddress();
			helperProcess.StartInfo.UseShellExecute = false;
			helperProcess.Start();

			Log.Info("Helper process was started successfully");

			var request = new HelloRequest
			{
				Name = "Client",
			};

			var response = await helloClient.SayHello(request, cancellationToken);

			Log.Info($"Hello response: '{response.Greeting}'");

			serverThread.Join();
		}

		private void RunIpcServer(CancellationToken cancellationToken)
		{
			ipcServer.Run(cancellationToken).GetAwaiter().GetResult();
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
