using System.Threading;
using System.Threading.Tasks;
using IpcDemo.Common.Contracts;

namespace IpcDemo.NetFrameworkHelper.Clients
{
	internal interface IHelloCallbackClient
	{
		Task<HelloCallbackResponse> Callback(HelloCallbackRequest request, CancellationToken cancellationToken);
	}
}
