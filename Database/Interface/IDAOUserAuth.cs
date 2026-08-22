using HMS_Temp_Humdity_Auth.Models.HMS_Temp_Humdity_Auth.Models;

namespace HMS_Temp_Humdity_Auth.Database.Interface
{
	public interface IDAOUserAuth
	{
		Task<UserAuthModel?> GetByUsernameAsync(string username);
		Task<UserAuthModel?> GetByUserIdAsync(string userId);
	}
}
