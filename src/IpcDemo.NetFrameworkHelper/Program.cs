using System;
using System.IO;
using System.Linq;
using System.Threading;
using IpcDemo.Common.Extensions;
using IpcDemo.Common.Interfaces;
using IpcDemo.NetFrameworkHelper.Clients;
using IpcDemo.NetFrameworkHelper.Controllers;
using log4net;
using log4net.Config;
using Microsoft.Extensions.DependencyInjection;

namespace IpcDemo.NetFrameworkHelper
{
	public static class Program
	{
		private static readonly ILog Log = LogManager.GetLogger("NetFrameworkHelper");

		public static int Main(string[] args)
		{
			try
			{
				XmlConfigurator.Configure(new FileInfo("log4net.config"));

				Log.Info(".NET Framework Helper has started");

				if (args.Length != 1)
				{
					Log.Error("Usage: IpcDemo.NetFrameworkHelper.exe <server address>");
					return 1;
				}

				var services = new ServiceCollection();

				services.AddIpcDemoServices();
				services.AddClientIpcChannel(args.Single());

				services.AddSingleton<IIpcController, HelloController>();
				services.AddSingleton<IHelloCallbackClient, HelloCallbackClient>();

				services.AddSingleton<ApplicationLogic>();

				using (var serviceProvider = services.BuildServiceProvider())
				{
					var applicationLogic = serviceProvider.GetRequiredService<ApplicationLogic>();
					applicationLogic.Run(CancellationToken.None).Wait();
				}

				Log.Info(".NET Framework Helper has finished");

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
