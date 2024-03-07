using System.Runtime.Serialization;

namespace IpcDemo.Common.Contracts
{
	[DataContract]
	public class HelloRequest
	{
		[DataMember(Order = 1)]
		public string Name { get; set; }
	}
}
