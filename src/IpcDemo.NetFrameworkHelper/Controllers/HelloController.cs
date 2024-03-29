using System;
using System.Threading;
using System.Threading.Tasks;
using IpcDemo.Common.Contracts;
using IpcDemo.Common.Extensions;
using IpcDemo.Common.Interfaces;
using IpcDemo.NetFrameworkHelper.Clients;
using log4net;
using static IpcDemo.Common.ControllerNames;
using static IpcDemo.Common.HelloControllerActions;

namespace IpcDemo.NetFrameworkHelper.Controllers
{
	internal class HelloController : IIpcController
	{
		private static readonly ILog Log = LogManager.GetLogger("HelloController");

		private readonly IHelloCallbackClient helloCallbackClient;

		public HelloController(IHelloCallbackClient helloCallbackClient)
		{
			this.helloCallbackClient = helloCallbackClient ?? throw new ArgumentNullException(nameof(helloCallbackClient));
		}

		public void RegisterActions(IIpcServer ipcServer)
		{
			ipcServer.RegisterAction<HelloRequest, HelloResponse>(HelloControllerName, SayHelloActionName, SayHello);
			ipcServer.RegisterAction<HowAreYouResponse>(HelloControllerName, HowAreYouActionName, HowAreYou);
		}

		public async Task<HelloResponse> SayHello(HelloRequest request, CancellationToken cancellationToken)
		{
			Log.Info($"Processing Hello request for '{request.Name}' ...");

			var callbackRequest = new HelloCallbackRequest
			{
				Greeting = "What is your name?",
			};

			var callbackResponse = await helloCallbackClient.Callback(callbackRequest, cancellationToken);

			Log.Info($"Response from callback: '{callbackResponse.Name}'");

			return new HelloResponse
			{
				Greeting = $"Hello, {callbackResponse.Name}!",
			};
		}

		public Task<HowAreYouResponse> HowAreYou(CancellationToken cancellationToken)
		{
			var response = new HowAreYouResponse
			{
				Status = "I am OK",
			};

			return Task.FromResult(response);
		}
	}
}
