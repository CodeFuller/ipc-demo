using System;
using System.Threading.Tasks;

namespace IpcDemo.Common.Interfaces
{
	internal interface IResponseDataProvider
	{
		Task<byte[]> ProvideResponseData(Guid requestId, Func<Task> clientCall);

		void PostResponseData(Guid requestId, byte[] responseData);
	}
}
