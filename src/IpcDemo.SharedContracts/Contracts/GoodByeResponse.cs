using System.Runtime.Serialization;

namespace IpcDemo.Common.Contracts
{
	[DataContract]
	public class GoodByeResponse
	{
		[DataMember(Order = 1)]
		public string Farewell { get; set; }
	}
}
