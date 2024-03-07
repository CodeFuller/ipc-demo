using System;
using System.IO;
using IpcDemo.Common.Interfaces;
using ProtoBuf;

namespace IpcDemo.Common
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

		public TData Deserialize<TData>(ReadOnlySpan<byte> data)
		{
			return Serializer.Deserialize<TData>(data);
		}
	}
}
