using System.Runtime.Serialization;

namespace IpcDemo.Common.Data
{
	[DataContract]
	internal class RequestDataContract
	{
		[DataMember(Order = 1)]
		public string ControllerName { get; set; }

		[DataMember(Order = 2)]
		public string ActionName { get; set; }

		[DataMember(Order = 3)]
		public byte[] Data { get; set; }
	}
}
