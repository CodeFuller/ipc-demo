using System;
using System.Threading;
using System.Threading.Tasks;

namespace IpcDemo.Common.Interfaces
{
	public interface IIpcServer
	{
		string GetAddress();

		void RegisterAction<TRequest, TResponse>(string controllerName, string actionName, Func<TRequest, CancellationToken, Task<TResponse>> action);

		Task Run(CancellationToken cancellationToken);
	}
}
