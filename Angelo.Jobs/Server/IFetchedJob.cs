using System;
using Angelo.Jobs.Models;

namespace Angelo.Jobs.Server
{
	public interface IFetchedJob : IDisposable
	{
		DelayedJob Job { get; }

		void RemoveFromQueue();
		void Requeue();
	}
}
