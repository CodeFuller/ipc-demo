using IpcDemo.Common.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace IpcDemo.NetCoreLauncher.Interfaces
{
	internal interface IGoodByeClient
	{
		Task<GoodByeResponse> SayGoodBye(GoodByeRequest request, CancellationToken cancellationToken);
	}
}
