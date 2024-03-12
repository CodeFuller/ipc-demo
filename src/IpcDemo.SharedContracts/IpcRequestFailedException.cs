using System;

namespace IpcDemo.Common
{
	public class IpcRequestFailedException : Exception
	{
		public override string StackTrace { get; }

		public IpcRequestFailedException(string message, string stackTrace)
			: base(message)
		{
			StackTrace = stackTrace;
		}
	}
}
