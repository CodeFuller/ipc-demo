using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace IpcDemo.NetCoreLauncher.Interfaces
{
	internal interface IChildProcessManager
	{
		Task RunChildProcess(Process process, CancellationToken cancellationToken);
	}
}
