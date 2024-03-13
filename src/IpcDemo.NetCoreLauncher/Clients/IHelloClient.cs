using System.Threading;
using System.Threading.Tasks;
using IpcDemo.Common.Contracts;

namespace IpcDemo.NetCoreLauncher.Clients
{
	internal interface IHelloClient
	{
		Task<HelloResponse> SayHello(HelloRequest request, CancellationToken cancellationToken);

		Task<HowAreYouResponse> HowAreYou(CancellationToken cancellationToken);
	}
}
