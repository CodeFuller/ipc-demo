using System;
using System.Runtime.Serialization;

namespace IpcDemo.Common.Data
{
	[DataContract]
	internal class IpcMessage
	{
		[DataMember(Order = 1)]
		public IpcMessageType MessageType { get; set; }

		[DataMember(Order = 2)]
		public Guid RequestId { get; set; }

		[DataMember(Order = 3)]
		public byte[] Data { get; set; }
	}
}
