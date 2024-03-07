using System.Threading;
using System.Threading.Tasks;

namespace IpcDemo.NetFrameworkHelper.Interfaces
{
	internal interface IIpcServer
	{
		Task Start(CancellationToken cancellationToken);
	}
}
