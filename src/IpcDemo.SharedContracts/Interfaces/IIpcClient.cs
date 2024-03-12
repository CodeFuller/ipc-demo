using System.Threading;
using System.Threading.Tasks;

namespace IpcDemo.Common.Interfaces
{
	public interface IIpcClient
	{
		Task<TResponse> CallServer<TRequest, TResponse>(string controllerName, string actionName, TRequest request, CancellationToken cancellationToken);
	}
}
