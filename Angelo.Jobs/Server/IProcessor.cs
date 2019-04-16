using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Angelo.Jobs.Server
{
	public interface IProcessor
	{
		Task ProcessAsync(ProcessingContext context);
	}
}
