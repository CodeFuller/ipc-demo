using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;
using IpcDemo.Common;
using IpcDemo.Common.Contracts;
using IpcDemo.Common.Extensions;
using IpcDemo.Common.Interfaces;
using IpcDemo.NetFrameworkHelper.Interfaces;
using log4net;

namespace IpcDemo.NetFrameworkHelper
{
	internal class NamedPipeIpcServer : IIpcServer
	{
		private static readonly ILog Log = LogManager.GetLogger("NamedPipeIpcServer");

		private readonly IDataSerializer dataSerializer;

		private readonly IHelloRequestProcessor helloRequestProcessor;

		private readonly IGoodByeRequestProcessor goodByeRequestProcessor;

		public NamedPipeIpcServer(IDataSerializer dataSerializer, IHelloRequestProcessor helloRequestProcessor, IGoodByeRequestProcessor goodByeRequestProcessor)
		{
			this.dataSerializer = dataSerializer ?? throw new ArgumentNullException(nameof(dataSerializer));
			this.helloRequestProcessor = helloRequestProcessor ?? throw new ArgumentNullException(nameof(helloRequestProcessor));
			this.goodByeRequestProcessor = goodByeRequestProcessor ?? throw new ArgumentNullException(nameof(goodByeRequestProcessor));
		}

		public async Task Start(CancellationToken cancellationToken)
		{
			Log.Info("Creating pipe stream ...");
			var pipeServerStream = new NamedPipeServerStream(Constants.PipeName, PipeDirection.InOut, 1);

			Log.Info("Waiting for connection ...");
			await pipeServerStream.WaitForConnectionAsync(cancellationToken);
			Log.Info("Client connected");

			using (var streamReader = new BinaryReader(pipeServerStream))
			using (var streamWriter = new BinaryWriter(pipeServerStream))
			{
				do
				{
					try
					{
						Log.Info("Waiting for request from client ...");
						var request = await ReadFromStream(streamReader, cancellationToken);
						Log.Info($"Got request from client: {request.RequestType}");

						switch (request.RequestType)
						{
							case RequestTypes.SayHello:
								await ProcessRequest<HelloRequest, HelloResponse>(request.RequestData, (r, ct) => helloRequestProcessor.ProcessRequest(r, ct), streamWriter, cancellationToken);
								break;

							case RequestTypes.SayGoodBye:
								await ProcessRequest<GoodByeRequest, GoodByeResponse>(request.RequestData, (r, ct) => goodByeRequestProcessor.ProcessRequest(r, ct), streamWriter, cancellationToken);
								break;

							default:
								throw new NotSupportedException($"Request type is not supported: {request.RequestType}");
						}
					}
					catch (Exception e)
					{
						Log.Error("Failed to read data from client", e);

						// TODO: Implement proper error handling.
						throw;
					}
					finally
					{
						// TODO: Close pipe properly.
					}
				}
				while (!cancellationToken.IsCancellationRequested);
			}
		}

		private static Task<(RequestTypes RequestType, byte[] RequestData)> ReadFromStream(BinaryReader streamReader, CancellationToken cancellationToken)
		{
			var requestType = (RequestTypes)streamReader.ReadInt32();
			var requestData = streamReader.ReadData();

			return Task.FromResult((requestType, requestData));
		}

		private async Task ProcessRequest<TRequest, TResponse>(byte[] requestData, Func<TRequest, CancellationToken, Task<TResponse>> requestProcessor, BinaryWriter streamWriter, CancellationToken cancellationToken)
		{
			var request = dataSerializer.Deserialize<TRequest>(requestData);

			var response = await requestProcessor(request, cancellationToken);

			streamWriter.WriteData(response, dataSerializer);
		}
	}
}
