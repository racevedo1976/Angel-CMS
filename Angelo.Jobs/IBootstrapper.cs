using System.Threading.Tasks;

namespace Angelo.Jobs
{
	public interface IBootstrapper
	{
		Task BootstrapAsync();
	}
}
