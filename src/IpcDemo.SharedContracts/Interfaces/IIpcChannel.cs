using System.Threading.Tasks;
using System.Threading;
using IpcDemo.Common.Data;

namespace IpcDemo.Common.Interfaces
{
	internal interface IIpcChannel
	{
		string GetAddress();

		Task WriteMessage(IpcMessage message, CancellationToken cancellationToken);

		Task<IpcMessage> ReadMessage(CancellationToken cancellationToken);
	}
}
