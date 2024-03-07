using IpcDemo.Common;
using System.Threading;
using System.Threading.Tasks;

namespace IpcDemo.NetCoreLauncher.Interfaces
{
	internal interface IIpcClient
	{
		Task<TResponse> CallServer<TRequest, TResponse>(RequestTypes requestType, TRequest request, CancellationToken cancellationToken);
	}
}
