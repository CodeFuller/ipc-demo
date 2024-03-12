using System;
using System.IO;
using IpcDemo.Common.Interfaces;
using ProtoBuf;

namespace IpcDemo.Common.Internal
{
	internal class ProtobufDataSerializer : IDataSerializer
	{
		public byte[] Serialize<TData>(TData data)
		{
			using (var stream = new MemoryStream())
			{
				Serializer.Serialize(stream, data);
				return stream.ToArray();
			}
		}

		public TData Deserialize<TData>(byte[] data)
		{
			return (TData)Deserialize(typeof(TData), data);
		}

		public object Deserialize(Type dataType, byte[] data)
		{
			using (var stream = new MemoryStream(data))
			{
				return Serializer.Deserialize(dataType, stream);
			}
		}
	}
}
