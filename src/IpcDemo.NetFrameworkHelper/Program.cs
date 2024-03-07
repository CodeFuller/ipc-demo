using System;
using System.IO;
using System.Threading;
using IpcDemo.Common.Extensions;
using IpcDemo.NetFrameworkHelper.Interfaces;
using IpcDemo.NetFrameworkHelper.RequestProcessors;
using log4net;
using log4net.Config;
using Microsoft.Extensions.DependencyInjection;

namespace IpcDemo.NetFrameworkHelper
{
	public static class Program
	{
		private static readonly ILog Log = LogManager.GetLogger("NetFrameworkHelper");

		public static void Main()
		{
			try
			{
				XmlConfigurator.Configure(new FileInfo("log4net.config"));

				Log.Info(".NET Framework Helper has started");

				var services = new ServiceCollection();

				services.AddIpcDemoServices();
				services.AddSingleton<IIpcServer, NamedPipeIpcServer>();
				services.AddSingleton<IHelloRequestProcessor, HelloRequestProcessor>();
				services.AddSingleton<IGoodByeRequestProcessor, GoodByeRequestProcessor>();
				services.AddSingleton<ApplicationLogic>();

				using (var serviceProvider = services.BuildServiceProvider())
				{
					var applicationLogic = serviceProvider.GetRequiredService<ApplicationLogic>();
					applicationLogic.Run(CancellationToken.None).Wait();
				}

				Log.Info(".NET Framework Helper has finished");
			}
			catch (Exception e)
			{
				Log.Fatal(e);
			}
		}
	}
}
