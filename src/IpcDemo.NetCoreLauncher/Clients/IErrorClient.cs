using System.Threading;
using System.Threading.Tasks;

namespace IpcDemo.NetCoreLauncher.Clients
{
	internal interface IErrorClient
	{
		Task TriggerError(CancellationToken cancellationToken);
	}
}
