using System.Runtime.Serialization;

namespace IpcDemo.Common.Contracts
{
	[DataContract]
	public class HowAreYouResponse
	{
		[DataMember(Order = 1)]
		public string Status { get; set; }
	}
}
