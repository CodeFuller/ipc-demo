using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IpcDemo.Common.Data;
using IpcDemo.Common.Interfaces;
using log4net;

namespace IpcDemo.Common.Internal
{
	internal class IpcServer : IIpcServer
	{
		private static readonly ILog Log = LogManager.GetLogger("IpcServer");

		private readonly struct ActionKey
		{
			public string ControllerName { get; }

			public string ActionName { get; }

			public ActionKey(string controllerName, string actionName)
			{
				ControllerName = controllerName ?? throw new ArgumentNullException(nameof(controllerName));
				ActionName = actionName ?? throw new ArgumentNullException(nameof(actionName));
			}
		}

		private class ActionDescriptor
		{
			public Type RequestType { get; }

			public Func<Object, CancellationToken, Task<Object>> Action { get; }

			public ActionDescriptor(Type requestType, Func<Object, CancellationToken, Task<Object>> action)
			{
				RequestType = requestType ?? throw new ArgumentNullException(nameof(requestType));
				Action = action ?? throw new ArgumentNullException(nameof(action));
			}
		}

		private readonly Dictionary<ActionKey, ActionDescriptor> registeredActions = new Dictionary<ActionKey, ActionDescriptor>();

		private readonly IIpcChannel ipcChannel;

		private readonly IDataSerializer dataSerializer;

		private readonly IResponseDataProvider _responseDataProvider;

		private readonly IReadOnlyCollection<IIpcController> controllers;

		public IpcServer(IIpcChannel ipcChannel, IDataSerializer dataSerializer, IResponseDataProvider responseDataProvider, IEnumerable<IIpcController> controllers)
		{
			this.ipcChannel = ipcChannel ?? throw new ArgumentNullException(nameof(ipcChannel));
			this.dataSerializer = dataSerializer ?? throw new ArgumentNullException(nameof(dataSerializer));
			this._responseDataProvider = responseDataProvider ?? throw new ArgumentNullException(nameof(responseDataProvider));
			this.controllers = controllers?.ToList() ?? throw new ArgumentNullException(nameof(controllers));
		}

		public string GetAddress()
		{
			return ipcChannel.GetAddress();
		}

		public void RegisterAction<TRequest, TResponse>(string controllerName, string actionName, Func<TRequest, CancellationToken, Task<TResponse>> action)
		{
			var actionKey = new ActionKey(controllerName, actionName);
			var actionDescriptor = new ActionDescriptor(typeof(TRequest), async (request, cancellationToken) => await action((TRequest)request, cancellationToken));

			registeredActions.Add(actionKey, actionDescriptor);
		}

		public async Task Run(CancellationToken cancellationToken)
		{
			Log.Info("Started IPC server");

			foreach (var controller in controllers)
			{
				controller.RegisterActions(this);
			}

			while (!cancellationToken.IsCancellationRequested)
			{
				var ipcMessage = await ipcChannel.ReadMessage(cancellationToken);

				switch (ipcMessage.MessageType)
				{
					case IpcMessageType.Request:
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
						Task.Run(() => ProcessRequest(ipcMessage, cancellationToken), cancellationToken);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
						break;

					case IpcMessageType.Response:
						ProcessResponse(ipcMessage);
						break;

					default:
						throw new NotSupportedException($"IPC message type is not supported: {ipcMessage.MessageType}");
				}
			}

			Log.Info("Finished IPC server");
		}

		// TODO: Handle exceptions for background tasks.
		private async Task ProcessRequest(IpcMessage requestMessage, CancellationToken cancellationToken)
		{
			var requestData = dataSerializer.Deserialize<RequestDataContract>(requestMessage.Data);

			Log.Debug($"Processing request: [Controller: {requestData.ControllerName}][Action: {requestData.ActionName}][Id: {requestMessage.RequestId}]");

			var actionKey = new ActionKey(requestData.ControllerName, requestData.ActionName);
			if (!registeredActions.TryGetValue(actionKey, out var actionDescriptor))
			{
				throw new NotSupportedException($"Action {actionKey.ActionName} from controller {actionKey.ControllerName} is not supported");
			}

			var request = dataSerializer.Deserialize(actionDescriptor.RequestType, requestData.Data);

			var response = await actionDescriptor.Action(request, cancellationToken);

			var responseMessage = new IpcMessage
			{
				MessageType = IpcMessageType.Response,
				RequestId = requestMessage.RequestId,
				Data = dataSerializer.Serialize(response),
			};

			await ipcChannel.WriteMessage(responseMessage, cancellationToken);

			Log.Debug($"Processed request: [Controller: {requestData.ControllerName}][Action: {requestData.ActionName}][Id: {requestMessage.RequestId}]");
		}

		private void ProcessResponse(IpcMessage responseMessage)
		{
			_responseDataProvider.PostResponseData(responseMessage.RequestId, responseMessage.Data);
		}
	}
}
