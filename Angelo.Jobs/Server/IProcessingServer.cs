using System;

namespace Angelo.Jobs.Server
{
	public interface IProcessingServer : IDisposable
	{
		void Pulse(PulseKind kind);
		void Start();
	}
}
