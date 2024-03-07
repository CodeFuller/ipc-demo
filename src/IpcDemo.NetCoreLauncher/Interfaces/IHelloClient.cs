using System.Threading;
using System.Threading.Tasks;
using IpcDemo.Common.Contracts;

namespace IpcDemo.NetCoreLauncher.Interfaces
{
	internal interface IHelloClient
	{
		Task<HelloResponse> SayHello(HelloRequest request, CancellationToken cancellationToken);
	}
}
