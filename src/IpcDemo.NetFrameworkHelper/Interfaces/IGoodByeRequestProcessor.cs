using System.Threading;
using System.Threading.Tasks;
using IpcDemo.Common.Contracts;

namespace IpcDemo.NetFrameworkHelper.Interfaces
{
	internal interface IGoodByeRequestProcessor
	{
		Task<GoodByeResponse> ProcessRequest(GoodByeRequest request, CancellationToken cancellationToken);
	}
}
