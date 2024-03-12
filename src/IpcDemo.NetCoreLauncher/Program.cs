using System;
using System.IO;
using System.Threading;
using IpcDemo.Common;
using IpcDemo.Common.Interfaces;
using IpcDemo.NetCoreLauncher.Clients;
using IpcDemo.NetCoreLauncher.Controllers;
using log4net;
using log4net.Config;
using Microsoft.Extensions.DependencyInjection;

namespace IpcDemo.NetCoreLauncher
{
	public static class Program
	{
		private static readonly ILog Log = LogManager.GetLogger("NetCoreLauncher");

		public static int Main()
		{
			try
			{
				XmlConfigurator.Configure(new FileInfo("log4net.config"));

				Log.Info(".NET Core Launcher has started");

				var services = new ServiceCollection();

				services.AddIpcDemoServices();
				services.AddServerIpcChannel();

				services.AddSingleton<IIpcController, HelloCallbackController>();
				services.AddSingleton<IHelloClient, HelloClient>();

				services.AddSingleton<ApplicationLogic>();

				using (var serviceProvider = services.BuildServiceProvider())
				{
					var applicationLogic = serviceProvider.GetRequiredService<ApplicationLogic>();
					applicationLogic.Run(CancellationToken.None).Wait();
				}

				Log.Info(".NET Core Launcher has finished");

				return 0;
			}
			catch (Exception e)
			{
				Log.Fatal(e);
				return e.HResult;
			}
		}
	}
}
