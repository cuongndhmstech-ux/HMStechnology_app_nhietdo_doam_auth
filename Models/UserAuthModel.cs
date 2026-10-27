using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HMS_Temp_Humdity_Auth.Models
{
	[BsonIgnoreExtraElements]
	public class UserAuthModel
	{
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string? Id { get; set; }

		public required string UserId { get; set; }

		public required string UserName { get; set; }

		public string? PhoneNumber { get; set; }

		public required string Password { get; set; }

		public string? CompanyId { get; set; }

		public CompanyPermission CompanyPermissions { get; set; }

		public Role Role { get; set; } = Role.Customer;
	}

	[Flags]
	public enum CompanyPermission : long
	{
		None = 0,

		UserManage = 1L << 0,
		DeviceManage = 1L << 1,
		LocationManage = 1L << 2,
		AlertManage = 1L << 3,
		Dashboard = 1L << 4,
		CompanyManage = 1L << 5,
		DevicePermissionManage = 1L << 6,

		Full = UserManage
			 | DeviceManage
			 | LocationManage
			 | AlertManage
			 | Dashboard
			 | CompanyManage | DevicePermissionManage
	}

	public enum Role
	{
		Admin,
		Customer
	}
}
