using System;
using System.Threading.Tasks;
using System.Threading;
using IpcDemo.Common.Extensions;
using IpcDemo.Common.Interfaces;
using static IpcDemo.Common.ControllerNames;
using static IpcDemo.Common.ErrorControllerActions;

namespace IpcDemo.NetFrameworkHelper.Controllers
{
	internal class ErrorController : IIpcController
	{
		public void RegisterActions(IIpcServer ipcServer)
		{
			ipcServer.RegisterAction(ErrorControllerName, ProduceErrorActionName, ProduceError);
		}

		public Task ProduceError(CancellationToken cancellationToken)
		{
			throw new InvalidOperationException("Oops");
		}
	}
}
