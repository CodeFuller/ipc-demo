using IpcDemo.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace IpcDemo.Common.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddIpcDemoServices(this IServiceCollection services)
		{
			services.AddSingleton<IDataSerializer, ProtobufDataSerializer>();

			return services;
		}
	}
}
