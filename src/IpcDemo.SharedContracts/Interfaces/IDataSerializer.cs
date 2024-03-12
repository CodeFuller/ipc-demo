using System;

namespace IpcDemo.Common.Interfaces
{
	public interface IDataSerializer
	{
		byte[] Serialize<TData>(TData data);

		TData Deserialize<TData>(byte[] data);

		Object Deserialize(Type dataType, byte[] data);
	}
}
