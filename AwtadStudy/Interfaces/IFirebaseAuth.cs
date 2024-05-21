using System;
using FirebaseAdmin.Auth;

namespace AwtadStudy.Interfaces
{
	public interface IFirebaseAuth
	{
        Task<UserRecord> GetUserAsync(string uid);
        Task<UserRecord> CreateUserAsync(UserRecordArgs userArgs);
        Task<UserRecord> UpdateUserAsync(UserRecordArgs newUserArgs);
        Task<IEnumerable<UserRecord>> GetUsersAsync();
        Task<string> DeleteUserAsync(string uid);
        Task<string> CreateCustomToken(string uid);
    }
}

