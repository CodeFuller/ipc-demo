using IpcDemo.Common.Interfaces;
using IpcDemo.Common.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace IpcDemo.Common.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddIpcDemoServices(this IServiceCollection services)
		{
			services.AddSingleton<IDataSerializer, ProtobufDataSerializer>();
			services.AddSingleton<IIpcServer, IpcServer>();
			services.AddSingleton<IIpcClient, IpcClient>();
			services.AddSingleton<IResponseDataProvider, ResponseDataProvider>();

			return services;
		}

		public static IServiceCollection AddServerIpcChannel(this IServiceCollection services)
		{
			services.AddSingleton<IIpcChannel>(PipeIpcChannel.CreateServerSideChannel());

			return services;
		}

		public static IServiceCollection AddClientIpcChannel(this IServiceCollection services, string serverAddress)
		{
			services.AddSingleton<IIpcChannel>(PipeIpcChannel.CreateClientSideChannel(serverAddress));

			return services;
		}
	}
}
