using System.Threading.Tasks;
using System.Threading;
using IpcDemo.Common.Contracts;
using IpcDemo.Common.Interfaces;

namespace IpcDemo.Common.Extensions
{
	public static class IpcClientExtensions
	{
		public static Task CallServer<TRequest>(this IIpcClient ipcClient, string controllerName, string actionName, TRequest request, CancellationToken cancellationToken)
		{
			return ipcClient.CallServer<TRequest, VoidDataContract>(controllerName, actionName, request, cancellationToken);
		}

		public static Task<TResponse> CallServer<TResponse>(this IIpcClient ipcClient, string controllerName, string actionName, CancellationToken cancellationToken)
		{
			return ipcClient.CallServer<VoidDataContract, TResponse>(controllerName, actionName, VoidDataContract.Instance, cancellationToken);
		}

		public static Task CallServer(this IIpcClient ipcClient, string controllerName, string actionName, CancellationToken cancellationToken)
		{
			return ipcClient.CallServer<VoidDataContract, VoidDataContract>(controllerName, actionName, VoidDataContract.Instance, cancellationToken);
		}
	}
}
