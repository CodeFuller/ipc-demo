using System.Runtime.Serialization;

namespace IpcDemo.Common.Data
{
	[DataContract]
	internal class ErrorDataContract
	{
		[DataMember(Order = 1)]
		public string ErrorMessage { get; set; }

		[DataMember(Order = 2)]
		public string StackTrace { get; set; }
	}
}
