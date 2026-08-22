namespace HMS_Temp_Humdity_Auth.Config
{
	public class MongoDbSettings
	{
		public string ConnectionString { get; set; } = string.Empty;

		public string DatabaseName { get; set; } = string.Empty;

		public string UserCollectionName { get; set; } = "Users";
	}
}
