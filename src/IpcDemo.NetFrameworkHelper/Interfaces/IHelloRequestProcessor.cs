using System.Threading;
using System.Threading.Tasks;
using IpcDemo.Common.Contracts;

namespace IpcDemo.NetFrameworkHelper.Interfaces
{
	internal interface IHelloRequestProcessor
	{
		Task<HelloResponse> ProcessRequest(HelloRequest request, CancellationToken cancellationToken);
	}
}
