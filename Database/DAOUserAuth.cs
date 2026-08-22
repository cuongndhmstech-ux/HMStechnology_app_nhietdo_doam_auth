using HMS_Temp_Humdity_Auth.Database.Interface;
using HMS_Temp_Humdity_Auth.Models.HMS_Temp_Humdity_Auth.Models;
using MongoDB.Driver;

namespace HMS_Temp_Humdity_Auth.Database
{
	public class DAOUserAuth : IDAOUserAuth
	{
		private readonly MongoDbContext _mongo;

		public DAOUserAuth(MongoDbContext mongo)
		{
			_mongo = mongo;
		}

		public async Task<UserAuthModel?> GetByUsernameAsync(string username)
		{
			if (string.IsNullOrWhiteSpace(username))
				return null;

			var normalizedUsername = username.Trim();

			var filter = Builders<UserAuthModel>.Filter.Eq(
				x => x.Username,
				normalizedUsername
			);

			return await _mongo.Users
				.Find(filter)
				.FirstOrDefaultAsync();
		}
		public async Task<UserAuthModel?> GetByUserIdAsync(string userId)
		{
			if (string.IsNullOrWhiteSpace(userId))
				return null;

			var filter = Builders<UserAuthModel>.Filter.Eq(
				x => x.UserId,
				userId
			);

			return await _mongo.Users
				.Find(filter)
				.FirstOrDefaultAsync();
		}
	}
}
