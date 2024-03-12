using System;
using System.Threading.Tasks;
using System.Threading;
using IpcDemo.Common.Interfaces;
using IpcDemo.Common.Contracts;

namespace IpcDemo.Common.Extensions
{
	public static class IpcServerExtensions
	{
		public static void RegisterAction<TRequest>(this IIpcServer ipcServer, string controllerName, string actionName, Func<TRequest, CancellationToken, Task> action)
		{
			async Task<VoidDataContract> ActionWithVoidResponse(TRequest request, CancellationToken cancellationToken)
			{
				await action(request, cancellationToken);

				return VoidDataContract.Instance;
			}

			ipcServer.RegisterAction<TRequest, VoidDataContract>(controllerName, actionName, ActionWithVoidResponse);
		}

		public static void RegisterAction<TResponse>(this IIpcServer ipcServer, string controllerName, string actionName, Func<CancellationToken, Task<TResponse>> action)
		{
			async Task<TResponse> ActionWithVoidRequest(VoidDataContract request, CancellationToken cancellationToken) => await action(cancellationToken);

			ipcServer.RegisterAction<VoidDataContract, TResponse>(controllerName, actionName, ActionWithVoidRequest);
		}

		public static void RegisterAction(this IIpcServer ipcServer, string controllerName, string actionName, Func<CancellationToken, Task> action)
		{
			async Task<VoidDataContract> ActionWithVoidRequestAndResponse(VoidDataContract request, CancellationToken cancellationToken)
			{
				await action(cancellationToken);

				return VoidDataContract.Instance;
			}

			ipcServer.RegisterAction<VoidDataContract, VoidDataContract>(controllerName, actionName, ActionWithVoidRequestAndResponse);
		}
	}
}
