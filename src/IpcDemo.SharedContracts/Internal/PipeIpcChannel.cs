using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;
using IpcDemo.Common.Data;
using IpcDemo.Common.Interfaces;

namespace IpcDemo.Common.Internal
{
	internal class PipeIpcChannel : IIpcChannel
	{
		private readonly BinaryReader streamReader;

		private readonly BinaryWriter streamWriter;

		private readonly IDataSerializer dataSerializer;

		private readonly string serverAddress;

		public static IIpcChannel CreateServerSideChannel()
		{
			var inputPipe = new AnonymousPipeServerStream(PipeDirection.In, HandleInheritability.Inheritable);
			var outputPipe = new AnonymousPipeServerStream(PipeDirection.Out, HandleInheritability.Inheritable);
			var dataSerializer = new ProtobufDataSerializer();

			var serverAddress = $"{inputPipe.GetClientHandleAsString()}-{outputPipe.GetClientHandleAsString()}";

			return new PipeIpcChannel(inputPipe, outputPipe, dataSerializer, serverAddress);
		}

		public static IIpcChannel CreateClientSideChannel(string serverAddress)
		{
			if (String.IsNullOrEmpty(serverAddress))
			{
				throw new ArgumentException("Server address could not be empty. It must contain 2 pipes handles.", nameof(serverAddress));
			}

			var pipeHandles = serverAddress.Split('-');
			if (pipeHandles.Length != 2)
			{
				throw new ArgumentException("Server address is invalid. It must contain 2 pipes handles.", nameof(serverAddress));
			}

			var inputPipe = new AnonymousPipeClientStream(PipeDirection.In, pipeHandles[1]);
			var outputPipe = new AnonymousPipeClientStream(PipeDirection.Out, pipeHandles[0]);
			var dataSerializer = new ProtobufDataSerializer();

			return new PipeIpcChannel(inputPipe, outputPipe, dataSerializer);
		}

		public PipeIpcChannel(PipeStream inputPipeStream, PipeStream outputPipeStream, IDataSerializer dataSerializer, string serverAddress)
			: this(inputPipeStream, outputPipeStream, dataSerializer)
		{
			this.serverAddress = serverAddress ?? throw new ArgumentNullException(nameof(serverAddress));
		}

		public PipeIpcChannel(PipeStream inputPipeStream, PipeStream outputPipeStream, IDataSerializer dataSerializer)
		{
			_ = inputPipeStream ?? throw new ArgumentNullException(nameof(inputPipeStream));

			streamReader = new BinaryReader(inputPipeStream);
			streamWriter = new BinaryWriter(outputPipeStream);
			this.dataSerializer = dataSerializer ?? throw new ArgumentNullException(nameof(dataSerializer));
		}

		public string GetAddress()
		{
			if (String.IsNullOrEmpty(serverAddress))
			{
				throw new InvalidOperationException("Cannot get address of client-side channel");
			}

			return serverAddress;
		}

		public Task WriteMessage(IpcMessage message, CancellationToken cancellationToken)
		{
			var serializedData = dataSerializer.Serialize(message);

			lock (streamWriter)
			{
				streamWriter.Write(serializedData.Length);
				streamWriter.Write(serializedData);
			}

			return Task.CompletedTask;
		}

		public Task<IpcMessage> ReadMessage(CancellationToken cancellationToken)
		{
			byte[] data;

			lock (streamReader)
			{
				var dataLength = streamReader.ReadInt32();
				data = streamReader.ReadBytes(dataLength);
			}

			var ipcMessage = dataSerializer.Deserialize<IpcMessage>(data);
			return Task.FromResult(ipcMessage);
		}
	}
}
