using System.Threading.Tasks;

namespace Angelo.Jobs
{
	public interface IJob
	{
		Task ExecuteAsync();
	}
}
