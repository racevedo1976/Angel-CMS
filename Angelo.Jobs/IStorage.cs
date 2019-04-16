using System.Threading.Tasks;

namespace Angelo.Jobs
{
	public interface IStorage
	{
		Task InitializeAsync();
		IStorageConnection GetConnection();
	}
}
