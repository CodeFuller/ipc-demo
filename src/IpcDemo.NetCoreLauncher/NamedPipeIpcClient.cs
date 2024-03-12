using System;
using System.IO;
using System.IO.Pipes;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using IpcDemo.Common;
using IpcDemo.Common.Extensions;
using IpcDemo.Common.Interfaces;
using IpcDemo.NetCoreLauncher.Interfaces;
using log4net;

namespace IpcDemo.NetCoreLauncher
{
	internal class NamedPipeIpcClient : IIpcClient
	{
		private static readonly ILog Log = LogManager.GetLogger("NamedPipeIpcClient");

		private readonly NamedPipeClientStream pipeClientStream;

		private readonly IDataSerializer dataSerializer;

		public NamedPipeIpcClient(IDataSerializer dataSerializer)
		{
			this.dataSerializer = dataSerializer ?? throw new ArgumentNullException(nameof(dataSerializer));

			pipeClientStream = new NamedPipeClientStream(".", Constants.PipeName, PipeDirection.InOut, PipeOptions.CurrentUserOnly, TokenImpersonationLevel.None);
		}

		public async Task<TResponse> CallServer<TRequest, TResponse>(RequestTypes requestType, TRequest request, CancellationToken cancellationToken)
		{
			if (!pipeClientStream.IsConnected)
			{
				Log.Info("Connecting to server ...");
				await pipeClientStream.ConnectAsync(cancellationToken);
				Log.Info("Connected to server");
			}

			var streamWriter = new BinaryWriter(pipeClientStream);

			Log.Info("Sending request to server ...");
			streamWriter.Write((int)requestType);
			streamWriter.WriteData(request, dataSerializer);
			Log.Info("Sent request to server");

			var streamReader = new BinaryReader(pipeClientStream);
			var data = streamReader.ReadData();
			var response = dataSerializer.Deserialize<TResponse>(data);

			return response;
		}
	}
}
