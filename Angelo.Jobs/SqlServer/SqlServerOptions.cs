namespace Angelo.Jobs
{
	public class SqlServerOptions
	{
		public string ConnectionString { get; set; }
        public string Schema { get; set; } = "jobs";
        public string KeyStoreTable { get; set; } = "Meta";
	}
}
