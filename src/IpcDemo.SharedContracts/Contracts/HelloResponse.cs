using System.Runtime.Serialization;

namespace IpcDemo.Common.Contracts
{
	[DataContract]
	public class HelloResponse
	{
		[DataMember(Order = 1)]
		public string Greeting { get; set; }
	}
}
