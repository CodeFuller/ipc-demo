using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;
using IpcDemo.Common.Interfaces;

namespace IpcDemo.Common.Internal
{
	internal class PipeIpcChannel : IIpcChannel
	{
		private readonly BinaryReader streamReader;

		private readonly BinaryWriter streamWriter;

		private readonly string serverAddress;

		public static IIpcChannel CreateServerSideChannel()
		{
			var inputPipe = new AnonymousPipeServerStream(PipeDirection.In, HandleInheritability.Inheritable);
			var outputPipe = new AnonymousPipeServerStream(PipeDirection.Out, HandleInheritability.Inheritable);
			var serverAddress = $"{inputPipe.GetClientHandleAsString()}-{outputPipe.GetClientHandleAsString()}";

			return new PipeIpcChannel(inputPipe, outputPipe, serverAddress);
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

			return new PipeIpcChannel(inputPipe, outputPipe, serverAddress);
		}

		public PipeIpcChannel(PipeStream inputPipeStream, PipeStream outputPipeStream, string serverAddress)
			: this(inputPipeStream, outputPipeStream)
		{
			this.serverAddress = serverAddress ?? throw new ArgumentNullException(nameof(serverAddress));
		}

		public PipeIpcChannel(PipeStream inputPipeStream, PipeStream outputPipeStream)
		{
			_ = inputPipeStream ?? throw new ArgumentNullException(nameof(inputPipeStream));

			streamReader = new BinaryReader(inputPipeStream);
			streamWriter = new BinaryWriter(outputPipeStream);
		}

		public string GetAddress()
		{
			if (String.IsNullOrEmpty(serverAddress))
			{
				throw new InvalidOperationException("Cannot get address of client-side channel");
			}

			return serverAddress;
		}

		public Task Write(int value, CancellationToken cancellationToken)
		{
			streamWriter.Write(value);
			return Task.CompletedTask;
		}

		public Task Write(byte[] data, CancellationToken cancellationToken)
		{
			streamWriter.Write(data);
			return Task.CompletedTask;
		}

		public Task<int> ReadInt32(CancellationToken cancellationToken)
		{
			var value = streamReader.ReadInt32();
			return Task.FromResult(value);
		}

		public Task<byte[]> ReadBytes(int length, CancellationToken cancellationToken)
		{
			var bytes = streamReader.ReadBytes(length);
			return Task.FromResult(bytes);
		}
	}
}
