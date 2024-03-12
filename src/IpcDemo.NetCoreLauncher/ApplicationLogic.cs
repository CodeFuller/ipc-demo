using System;
using System.Threading;
using System.Threading.Tasks;
using IpcDemo.Common.Contracts;
using IpcDemo.NetCoreLauncher.Interfaces;
using log4net;

namespace IpcDemo.NetCoreLauncher
{
	internal class ApplicationLogic
	{
		private static readonly ILog Log = LogManager.GetLogger("ApplicationLogic");

		private readonly IHelloClient helloClient;

		private readonly IGoodByeClient goodByeClient;

		public ApplicationLogic(IHelloClient helloClient, IGoodByeClient goodByeClient)
		{
			this.helloClient = helloClient ?? throw new ArgumentNullException(nameof(helloClient));
			this.goodByeClient = goodByeClient ?? throw new ArgumentNullException(nameof(goodByeClient));
		}

		public async Task Run(CancellationToken cancellationToken)
		{
			var helloRequest = new HelloRequest
			{
				Name = "CodeFuller",
			};

			Log.Info("Sending SayHello request ...");
			var helloResponse = await helloClient.SayHello(helloRequest, cancellationToken);
			Log.Info($"SayHello response: '{helloResponse.Greeting}'");

			var goodByeRequest = new GoodByeRequest
			{
				Name = "CodeFuller",
			};

			Log.Info("Sending SayGoodBye request ...");
			var goodByeResponse = await goodByeClient.SayGoodBye(goodByeRequest, cancellationToken);
			Log.Info($"SayGoodBye response: '{goodByeResponse.Farewell}'");
		}
	}
}
