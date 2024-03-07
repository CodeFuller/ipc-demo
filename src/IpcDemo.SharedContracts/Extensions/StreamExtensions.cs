using System.IO;
using IpcDemo.Common.Interfaces;

namespace IpcDemo.Common.Extensions
{
	public static class StreamExtensions
	{
		public static byte[] ReadData(this BinaryReader streamReader)
		{
			var dataLength = streamReader.ReadInt32();
			var requestData = streamReader.ReadBytes(dataLength);

			return requestData;
		}

		public static void WriteData<TData>(this BinaryWriter binaryWriter, TData data, IDataSerializer dataSerializer)
		{
			var serializedData = dataSerializer.Serialize(data);

			binaryWriter.Write(serializedData.Length);
			binaryWriter.Write(serializedData);
		}
	}
}
