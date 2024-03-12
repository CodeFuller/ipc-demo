using System.Threading.Tasks;
using System.Threading;

namespace IpcDemo.Common.Interfaces
{
	internal interface IIpcChannel
	{
		string GetAddress();

		Task Write(int value, CancellationToken cancellationToken);

		Task Write(byte[] data, CancellationToken cancellationToken);

		Task<int> ReadInt32(CancellationToken cancellationToken);

		Task<byte[]> ReadBytes(int length, CancellationToken cancellationToken);
	}
}
