using System;
using System.Threading;
using System.Threading.Tasks;
using IpcDemo.Common.Data;
using IpcDemo.Common.Extensions;
using IpcDemo.Common.Interfaces;
using log4net;

namespace IpcDemo.Common.Internal
{
	internal class IpcClient : IIpcClient
	{
		private static readonly ILog Log = LogManager.GetLogger("IpcClient");

		private readonly IIpcChannel ipcChannel;

		private readonly IDataSerializer dataSerializer;

		private readonly IResponseDataProvider _responseDataProvider;

		public IpcClient(IIpcChannel ipcChannel, IDataSerializer dataSerializer, IResponseDataProvider responseDataProvider)
		{
			this.ipcChannel = ipcChannel ?? throw new ArgumentNullException(nameof(ipcChannel));
			this.dataSerializer = dataSerializer ?? throw new ArgumentNullException(nameof(dataSerializer));
			this._responseDataProvider = responseDataProvider ?? throw new ArgumentNullException(nameof(responseDataProvider));
		}

		public async Task<TResponse> CallServer<TRequest, TResponse>(string controllerName, string actionName, TRequest request, CancellationToken cancellationToken)
		{
			var requestId = Guid.NewGuid();

			var requestData = new RequestDataContract
			{
				ControllerName = controllerName,
				ActionName = actionName,
				Data = dataSerializer.Serialize(request),
			};

			var requestMessage = new IpcMessage
			{
				MessageType = IpcMessageType.Request,
				RequestId = requestId,
				Data = dataSerializer.Serialize(requestData),
			};

			Task ClientCall()
			{
				Log.Debug($"Writing request message ({typeof(TRequest).Name}) to the channel ...");
				return ipcChannel.WriteData(requestMessage, dataSerializer, cancellationToken);
			}

			var responseData = await _responseDataProvider.ProvideResponseData(requestId, ClientCall);

			return dataSerializer.Deserialize<TResponse>(responseData);
		}
	}
}
