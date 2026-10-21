using HMS_Temp_Humdity_Auth.Config;
using HMS_Temp_Humdity_Auth.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace HMS_Temp_Humdity_Auth.Database
{
	public class MongoDbContext
	{
		private readonly IMongoDatabase _database;
		private readonly MongoDbSettings _settings;

		public MongoDbContext(IOptions<MongoDbSettings> options)
		{
			_settings = options.Value;

			var client = new MongoClient(_settings.ConnectionString);

			_database = client.GetDatabase(_settings.DatabaseName);
		}

		public IMongoDatabase Database => _database;

		public IMongoCollection<UserAuthModel> Users => _database.GetCollection<UserAuthModel>(_settings.UserCollectionName);
	}
}
