using System.Threading;
using System.Threading.Tasks;
using IpcDemo.Common.Interfaces;

namespace IpcDemo.Common.Extensions
{
	internal static class IpcChannelExtensions
	{
		public static async Task<TData> ReadData<TData>(this IIpcChannel ipcChannel, IDataSerializer dataSerializer, CancellationToken cancellationToken)
		{
			var dataLength = await ipcChannel.ReadInt32(cancellationToken);
			var data = await ipcChannel.ReadBytes(dataLength, cancellationToken);

			return dataSerializer.Deserialize<TData>(data);
		}

		public static async Task WriteData<TData>(this IIpcChannel ipcChannel, TData data, IDataSerializer dataSerializer, CancellationToken cancellationToken)
		{
			var serializedData = dataSerializer.Serialize(data);

			await ipcChannel.Write(serializedData.Length, cancellationToken);
			await ipcChannel.Write(serializedData, cancellationToken);
		}
	}
}
