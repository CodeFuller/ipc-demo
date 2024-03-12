using System.Runtime.Serialization;

namespace IpcDemo.Common.Contracts
{
	[DataContract]
	internal class VoidDataContract
	{
		public static VoidDataContract Instance { get; } = new VoidDataContract();
	}
}
