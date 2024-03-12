using System.Runtime.Serialization;

namespace IpcDemo.Common.Contracts
{
	[DataContract]
	public class HelloCallbackRequest
	{
		[DataMember(Order = 1)]
		public string Greeting { get; set; }
	}
}
